using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API.Controllers
{
    [Route("api/events/{eventId}/[controller]")]
    [ApiController]
    [Authorize]
    public class RegistrationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegistrationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterForEvent(int eventId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            if (await _context.EventRegistrations.AnyAsync(er => er.EventId == eventId && er.UserId == userId))
                return BadRequest(new { message = "Already registered for this event" });

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

            return Ok(new { message = "Successfully registered for the event" });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<ActionResult> GetEventParticipants(int eventId)
        {
            var participants = await _context.EventRegistrations
                .Where(er => er.EventId == eventId)
                .Include(er => er.User)
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
