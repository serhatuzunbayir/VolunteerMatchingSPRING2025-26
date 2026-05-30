using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Data.Models
{
    /// <summary>
    /// Represents a virtual event on the platform.
    /// Events can be Active, Completed, or Cancelled.
    /// Registration is blocked when Status != "Active" or Registrations.Count >= Capacity.
    /// </summary>
    public class Event
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Short, descriptive title shown in event listings.</summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>Full description of the event shown on the details page.</summary>
        [Required]
        public string Description { get; set; }

        /// <summary>Scheduled date and time of the event.</summary>
        [Required]
        public DateTime DateTime { get; set; }

        /// <summary>Physical or virtual location (e.g., a meeting link or room name).</summary>
        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        /// <summary>Maximum number of attendees allowed. Must be >= current registration count.</summary>
        [Required]
        public int Capacity { get; set; }

        /// <summary>ID of the Admin or Staff user who created this event.</summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>Lifecycle status: "Active", "Completed", or "Cancelled".</summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        /// <summary>Timestamp of when the event record was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>All registrations for this event (navigation property).</summary>
        public virtual ICollection<EventRegistration> Registrations { get; set; }
    }
}
