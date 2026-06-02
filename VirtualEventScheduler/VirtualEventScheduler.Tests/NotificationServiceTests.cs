using Microsoft.Extensions.Logging;
using Moq;
using VirtualEventScheduler.API.Services;
using Xunit;

namespace VirtualEventScheduler.Tests
{
    /// <summary>
    /// Tests for the delegate-based NotificationService.
    /// Verifies that each delegate (OnEventCreated, OnUserRegistered, OnEventCancelled)
    /// is correctly fired when the corresponding Notify method is called.
    /// </summary>
    public class NotificationServiceTests
    {
        // Helper: creates a NotificationService with a mocked logger
        private NotificationService CreateService()
        {
            var logger = new Mock<ILogger<NotificationService>>();
            return new NotificationService(logger.Object);
        }

        // Test 1 ─────────────────────────────────────────────────────────────
        [Fact]
        public void NotifyEventCreated_ShouldFireOnEventCreatedDelegate()
        {
            // Arrange
            var service = CreateService();
            bool wasFired = false;
            service.OnEventCreated += (title, date, createdBy) => wasFired = true;

            // Act
            service.NotifyEventCreated("Tech Talk 2026", DateTime.Now.AddDays(7), "Admin User");

            // Assert
            Assert.True(wasFired, "OnEventCreated delegate should have fired.");
        }

        // Test 2 ─────────────────────────────────────────────────────────────
        [Fact]
        public void NotifyEventCreated_ShouldPassCorrectTitleToDelegate()
        {
            // Arrange
            var service = CreateService();
            string? capturedTitle = null;
            service.OnEventCreated += (title, date, createdBy) => capturedTitle = title;

            // Act
            service.NotifyEventCreated("SE 410 Demo Day", DateTime.Now.AddDays(1), "Staff Member");

            // Assert
            Assert.Equal("SE 410 Demo Day", capturedTitle);
        }

        // Test 3 ─────────────────────────────────────────────────────────────
        [Fact]
        public void NotifyUserRegistered_ShouldFireAndCaptureUserEmail()
        {
            // Arrange
            var service = CreateService();
            string? capturedEmail = null;
            service.OnUserRegistered += (name, email, eventTitle, date) => capturedEmail = email;

            // Act
            service.NotifyUserRegistered("Ali Yilmaz", "ali@test.com", "Tech Talk 2026", DateTime.Now.AddDays(3));

            // Assert
            Assert.Equal("ali@test.com", capturedEmail);
        }

        // Test 4 ─────────────────────────────────────────────────────────────
        [Fact]
        public void NotifyEventCancelled_ShouldFireOnEventCancelledDelegate()
        {
            // Arrange
            var service = CreateService();
            string? capturedTitle = null;
            service.OnEventCancelled += (title, cancelledBy) => capturedTitle = title;

            // Act
            service.NotifyEventCancelled("Cancelled Workshop", "Admin User");

            // Assert
            Assert.Equal("Cancelled Workshop", capturedTitle);
        }

        // Test 5 ─────────────────────────────────────────────────────────────
        [Fact]
        public void NotifyEventUpdated_ShouldFireOnEventUpdatedDelegate()
        {
            // Arrange
            var service = CreateService();
            bool wasFired = false;
            service.OnEventUpdated += (title, updatedBy) => wasFired = true;

            // Act
            service.NotifyEventUpdated("Updated Event", "Staff Member");

            // Assert
            Assert.True(wasFired, "OnEventUpdated delegate should have fired.");
        }
    }
}
