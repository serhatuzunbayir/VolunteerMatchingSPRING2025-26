using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VirtualEventScheduler.API.DTOs;
using VirtualEventScheduler.API.Services;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API.Controllers
{
    /// <summary>
    /// Handles all event-related operations: listing, filtering (LINQ), creating, updating, and cancelling events.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public EventsController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Returns all events, optionally filtered by date range and status using LINQ.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            // Build LINQ query with optional filters
            var query = _context.Events
                .Include(e => e.Registrations)
                .AsQueryable();

            // Filter by start date if provided
            if (startDate.HasValue)
                query = query.Where(e => e.DateTime >= startDate.Value);

            // Filter by end date if provided
            if (endDate.HasValue)
                query = query.Where(e => e.DateTime <= endDate.Value);

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);

            var events = await query
                .OrderBy(e => e.DateTime)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    Location = e.Location,
                    Capacity = e.Capacity,
                    Status = e.Status,
                    RegisteredCount = e.Registrations.Count,
                    IsFull = e.Registrations.Count >= e.Capacity
                })
                .ToListAsync();

            return Ok(events);
        }

        /// <summary>Returns a single event by its ID.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
                return NotFound();

            var eventDto = new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                DateTime = eventItem.DateTime,
                Location = eventItem.Location,
                Capacity = eventItem.Capacity,
                Status = eventItem.Status,
                RegisteredCount = eventItem.Registrations.Count,
                IsFull = eventItem.Registrations.Count >= eventItem.Capacity
            };

            return Ok(eventDto);
        }

        /// <summary>
        /// Returns all events that the currently authenticated user is registered for.
        /// Uses LINQ to filter registrations by user ID.
        /// </summary>
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetMyEvents()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // LINQ: join registrations to events and filter by current user
            var myEvents = await _context.EventRegistrations
                .Where(er => er.UserId == userId)
                .Include(er => er.Event)
                    .ThenInclude(e => e.Registrations)
                .Select(er => new EventDto
                {
                    Id = er.Event.Id,
                    Title = er.Event.Title,
                    Description = er.Event.Description,
                    DateTime = er.Event.DateTime,
                    Location = er.Event.Location,
                    Capacity = er.Event.Capacity,
                    Status = er.Event.Status,
                    RegisteredCount = er.Event.Registrations.Count,
                    IsFull = er.Event.Registrations.Count >= er.Event.Capacity
                })
                .OrderBy(e => e.DateTime)
                .ToListAsync();

            return Ok(myEvents);
        }

        /// <summary>Creates a new event. Only Admin and Staff may create events.</summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(EventCreateDto eventCreateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var newEvent = new Event
            {
                Title = eventCreateDto.Title,
                Description = eventCreateDto.Description,
                DateTime = eventCreateDto.DateTime,
                Location = eventCreateDto.Location,
                Capacity = eventCreateDto.Capacity,
                CreatedBy = userId,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            // Retrieve the creator's name for the notification
            var creator = await _context.Users.FindAsync(userId);
            _notificationService.NotifyEventCreated(newEvent.Title, newEvent.DateTime, creator?.FullName ?? "Unknown");

            var eventDto = new EventDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                DateTime = newEvent.DateTime,
                Location = newEvent.Location,
                Capacity = newEvent.Capacity,
                Status = newEvent.Status,
                RegisteredCount = 0,
                IsFull = false
            };

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, eventDto);
        }

        /// <summary>
        /// Updates an existing event's details. Only Admin and Staff may update events.
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EventDto>> UpdateEvent(int id, EventUpdateDto updateDto)
        {
            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            if (eventItem.Status == "Cancelled")
                return BadRequest(new { message = "Cannot update a cancelled event" });

            // Apply updates
            eventItem.Title = updateDto.Title;
            eventItem.Description = updateDto.Description;
            eventItem.DateTime = updateDto.DateTime;
            eventItem.Location = updateDto.Location;

            // Ensure new capacity is not less than current registrations
            if (updateDto.Capacity < eventItem.Registrations.Count)
                return BadRequest(new { message = $"Capacity cannot be less than the current registration count ({eventItem.Registrations.Count})" });

            eventItem.Capacity = updateDto.Capacity;

            await _context.SaveChangesAsync();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updater = userIdClaim != null ? await _context.Users.FindAsync(int.Parse(userIdClaim)) : null;
            _notificationService.NotifyEventUpdated(eventItem.Title, updater?.FullName ?? "Unknown");

            return Ok(new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                DateTime = eventItem.DateTime,
                Location = eventItem.Location,
                Capacity = eventItem.Capacity,
                Status = eventItem.Status,
                RegisteredCount = eventItem.Registrations.Count,
                IsFull = eventItem.Registrations.Count >= eventItem.Capacity
            });
        }

        /// <summary>
        /// Cancels an event. Only Admin and Staff may cancel events.
        /// Fires the OnEventCancelled delegate to notify registered handlers.
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            if (eventItem.Status == "Cancelled")
                return BadRequest(new { message = "Event is already cancelled" });

            // Mark as cancelled
            eventItem.Status = "Cancelled";
            await _context.SaveChangesAsync();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var canceller = userIdClaim != null ? await _context.Users.FindAsync(int.Parse(userIdClaim)) : null;

            // Fire the cancellation delegate
            _notificationService.NotifyEventCancelled(eventItem.Title, canceller?.FullName ?? "Unknown");

            return Ok(new { message = $"Event '{eventItem.Title}' has been cancelled" });
        }
    }
}
