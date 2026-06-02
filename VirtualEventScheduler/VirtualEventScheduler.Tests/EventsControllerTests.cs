using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using VirtualEventScheduler.API.Controllers;
using VirtualEventScheduler.API.DTOs;
using VirtualEventScheduler.API.Services;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;
using Xunit;

namespace VirtualEventScheduler.Tests
{
    /// <summary>
    /// Unit tests for EventsController.
    /// Uses an in-memory SQLite database so no real DB connection is needed.
    /// Covers LINQ filtering, event creation, cancellation, and update.
    /// </summary>
    public class EventsControllerTests
    {
        // Creates a fresh in-memory database for each test (unique name = isolation)
        private AppDbContext CreateDb(string testName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: testName)
                .Options;
            var db = new AppDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an EventsController wired to the given DB, acting as a specific user
        private EventsController CreateController(AppDbContext db, int userId = 1, string role = "Admin")
        {
            var logger = new Mock<ILogger<NotificationService>>();
            var notificationService = new NotificationService(logger.Object);

            var controller = new EventsController(db, notificationService);

            // Simulate a logged-in user with the given userId and role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
                }
            };

            return controller;
        }

        // Test 6 ─────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetEvents_WhenNoEventsExist_ReturnsEmptyList()
        {
            // Arrange
            using var db = CreateDb(nameof(GetEvents_WhenNoEventsExist_ReturnsEmptyList));
            var controller = CreateController(db);

            // Act
            var result = await controller.GetEvents(null, null, null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var events = Assert.IsAssignableFrom<IEnumerable<EventDto>>(ok.Value);
            Assert.Empty(events);
        }

        // Test 7 ─────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetEvents_WithStatusFilter_ReturnsOnlyMatchingEvents()
        {
            // Arrange – seed two events with different statuses
            // Note: User Id=1 is already added by OnModelCreating HasData (admin seed)
            using var db = CreateDb(nameof(GetEvents_WithStatusFilter_ReturnsOnlyMatchingEvents));
            db.Events.Add(new Event { Title = "Active Event",    DateTime = DateTime.Now.AddDays(1), Location = "Online", Capacity = 10, CreatedBy = 1, Status = "Active",    Description = "desc" });
            db.Events.Add(new Event { Title = "Cancelled Event", DateTime = DateTime.Now.AddDays(2), Location = "Online", Capacity = 10, CreatedBy = 1, Status = "Cancelled", Description = "desc" });
            await db.SaveChangesAsync();

            var controller = CreateController(db);

            // Act – LINQ filter by status = "Active"
            var result = await controller.GetEvents(null, null, "Active");

            // Assert – only the Active event should be returned
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var events = Assert.IsAssignableFrom<IEnumerable<EventDto>>(ok.Value).ToList();
            Assert.Single(events);
            Assert.Equal("Active Event", events[0].Title);
        }

        // Test 8 ─────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateEvent_ShouldPersistEventToDatabase()
        {
            // Arrange – seeded admin (Id=1) is already in DB via HasData
            using var db = CreateDb(nameof(CreateEvent_ShouldPersistEventToDatabase));

            var controller = CreateController(db, userId: 1, role: "Admin");
            var dto = new EventCreateDto
            {
                Title       = "New Workshop",
                Description = "A coding workshop",
                DateTime    = DateTime.Now.AddDays(5),
                Location    = "Room 101",
                Capacity    = 25
            };

            // Act
            var result = await controller.CreateEvent(dto);

            // Assert – event was saved to DB
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, await db.Events.CountAsync());
            Assert.Equal("New Workshop", (await db.Events.FirstAsync()).Title);
        }

        // Test 9 ─────────────────────────────────────────────────────────────
        [Fact]
        public async Task CancelEvent_ShouldChangeStatusToCancelled()
        {
            // Arrange – seeded admin (Id=1) already exists
            using var db = CreateDb(nameof(CancelEvent_ShouldChangeStatusToCancelled));
            db.Events.Add(new Event { Id = 10, Title = "Live Event", DateTime = DateTime.Now.AddDays(3), Location = "Online", Capacity = 20, CreatedBy = 1, Status = "Active", Description = "desc" });
            await db.SaveChangesAsync();

            var controller = CreateController(db, userId: 1, role: "Admin");

            // Act
            var result = await controller.CancelEvent(10);

            // Assert – status must be "Cancelled" in the database
            Assert.IsType<OkObjectResult>(result);
            var updatedEvent = await db.Events.FindAsync(10);
            Assert.Equal("Cancelled", updatedEvent!.Status);
        }

        // Test 10 ─────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetEvent_WithInvalidId_ReturnsNotFound()
        {
            // Arrange – empty database, no events
            using var db = CreateDb(nameof(GetEvent_WithInvalidId_ReturnsNotFound));
            var controller = CreateController(db);

            // Act
            var result = await controller.GetEvent(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
