using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VirtualEventScheduler.API.Services;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API.Controllers
{
    /// <summary>
    /// Manages event registrations: registering, unregistering, and viewing participants.
    /// Uses delegates via NotificationService to fire registration events.
    /// </summary>
    [Route("api/events/{eventId}/[controller]")]
    [ApiController]
    [Authorize]
    public class RegistrationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public RegistrationsController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Registers the currently authenticated user for a specific event.
        /// Fires the OnUserRegistered delegate after a successful registration.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RegisterForEvent(int eventId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Load event with current registrations
            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            if (eventItem.Status == "Cancelled")
                return BadRequest(new { message = "Cannot register for a cancelled event" });

            // Check for duplicate registration using LINQ
            if (await _context.EventRegistrations.AnyAsync(er => er.EventId == eventId && er.UserId == userId))
                return BadRequest(new { message = "Already registered for this event" });

            // Check if the event is already full
            if (eventItem.Registrations.Count >= eventItem.Capacity)
                return BadRequest(new { message = "Event is full" });

            var registration = new EventRegistration
            {
                EventId = eventId,
                UserId = userId,
                RegisteredAt = DateTime.Now
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            // Fire the registration delegate to notify all subscribed handlers
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
                _notificationService.NotifyUserRegistered(user.FullName, user.Email, eventItem.Title, eventItem.DateTime);

            return Ok(new { message = "Successfully registered for the event" });
        }

        /// <summary>
        /// Unregisters the currently authenticated user from an event.
        /// Admins and Staff can also unregister any user by passing a userId query parameter.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> UnregisterFromEvent(int eventId, [FromQuery] int? userId = null)
        {
            var callerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (callerIdClaim == null)
                return Unauthorized();

            int callerId = int.Parse(callerIdClaim);
            string callerRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            // Determine which user to unregister
            int targetUserId = callerId;
            if (userId.HasValue && (callerRole == "Admin" || callerRole == "Staff"))
                targetUserId = userId.Value;

            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(er => er.EventId == eventId && er.UserId == targetUserId);

            if (registration == null)
                return NotFound(new { message = "Registration not found" });

            _context.EventRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unregistered from the event" });
        }

        /// <summary>
        /// Returns the list of all participants registered for an event.
        /// Only accessible by Admin and Staff roles.
        /// Uses LINQ to select and sort participant data.
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<ActionResult> GetEventParticipants(int eventId)
        {
            // Verify the event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
                return NotFound(new { message = "Event not found" });

            // LINQ: filter registrations by event, include user data, sort by registration date
            var participants = await _context.EventRegistrations
                .Where(er => er.EventId == eventId)
                .Include(er => er.User)
                .OrderBy(er => er.RegisteredAt)
                .Select(er => new
                {
                    er.User.Id,
                    er.User.FullName,
                    er.User.Email,
                    er.RegisteredAt
                })
                .ToListAsync();

            return Ok(participants);
        }
    }
}
