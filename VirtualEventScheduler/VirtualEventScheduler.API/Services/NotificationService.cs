namespace VirtualEventScheduler.API.Services
{
    // Delegate type for event creation notifications
    public delegate void EventCreatedDelegate(string eventTitle, DateTime eventDate, string createdByName);

    // Delegate type for user registration notifications
    public delegate void UserRegisteredDelegate(string userFullName, string userEmail, string eventTitle, DateTime eventDate);

    // Delegate type for event cancellation notifications
    public delegate void EventCancelledDelegate(string eventTitle, string cancelledByName);

    // Delegate type for event update notifications
    public delegate void EventUpdatedDelegate(string eventTitle, string updatedByName);

    /// <summary>
    /// Handles all notification events for the Virtual Event Scheduler using delegates and events.
    /// Registered handlers are invoked when events occur (creation, registration, cancellation, update).
    /// </summary>
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        // Events backed by the custom delegate types
        public event EventCreatedDelegate? OnEventCreated;
        public event UserRegisteredDelegate? OnUserRegistered;
        public event EventCancelledDelegate? OnEventCancelled;
        public event EventUpdatedDelegate? OnEventUpdated;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;

            // Register default logging handlers on startup
            OnEventCreated += LogEventCreated;
            OnUserRegistered += LogUserRegistered;
            OnEventCancelled += LogEventCancelled;
            OnEventUpdated += LogEventUpdated;
        }

        /// <summary>Fires the OnEventCreated event to notify all registered handlers.</summary>
        public void NotifyEventCreated(string eventTitle, DateTime eventDate, string createdByName)
        {
            OnEventCreated?.Invoke(eventTitle, eventDate, createdByName);
        }

        /// <summary>Fires the OnUserRegistered event to notify all registered handlers.</summary>
        public void NotifyUserRegistered(string userFullName, string userEmail, string eventTitle, DateTime eventDate)
        {
            OnUserRegistered?.Invoke(userFullName, userEmail, eventTitle, eventDate);
        }

        /// <summary>Fires the OnEventCancelled event to notify all registered handlers.</summary>
        public void NotifyEventCancelled(string eventTitle, string cancelledByName)
        {
            OnEventCancelled?.Invoke(eventTitle, cancelledByName);
        }

        /// <summary>Fires the OnEventUpdated event to notify all registered handlers.</summary>
        public void NotifyEventUpdated(string eventTitle, string updatedByName)
        {
            OnEventUpdated?.Invoke(eventTitle, updatedByName);
        }

        // Default handler: logs event creation
        private void LogEventCreated(string eventTitle, DateTime eventDate, string createdByName)
        {
            _logger.LogInformation(
                "[NOTIFICATION] Event created: '{Title}' on {Date} by {User}",
                eventTitle, eventDate.ToString("yyyy-MM-dd HH:mm"), createdByName);
        }

        // Default handler: logs user registration for an event
        private void LogUserRegistered(string userFullName, string userEmail, string eventTitle, DateTime eventDate)
        {
            _logger.LogInformation(
                "[NOTIFICATION] Registration confirmed: {Name} ({Email}) registered for '{Event}' on {Date}",
                userFullName, userEmail, eventTitle, eventDate.ToString("yyyy-MM-dd HH:mm"));
        }

        // Default handler: logs event cancellation
        private void LogEventCancelled(string eventTitle, string cancelledByName)
        {
            _logger.LogInformation(
                "[NOTIFICATION] Event cancelled: '{Title}' by {User}",
                eventTitle, cancelledByName);
        }

        // Default handler: logs event update
        private void LogEventUpdated(string eventTitle, string updatedByName)
        {
            _logger.LogInformation(
                "[NOTIFICATION] Event updated: '{Title}' by {User}",
                eventTitle, updatedByName);
        }
    }
}
