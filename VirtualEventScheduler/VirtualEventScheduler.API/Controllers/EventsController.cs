using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VirtualEventScheduler.API.DTOs;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            var query = _context.Events
                .Include(e => e.Registrations)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.DateTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.DateTime <= endDate.Value);

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

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(EventCreateDto eventCreateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var newEvent = new Event
            {
                Title = eventCreateDto.Title,
                Description = eventCreateDto.Description,
                DateTime = eventCreateDto.DateTime,
                Location = eventCreateDto.Location,
                Capacity = eventCreateDto.Capacity,
                CreatedBy = int.Parse(userIdClaim),
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

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
    }
}
