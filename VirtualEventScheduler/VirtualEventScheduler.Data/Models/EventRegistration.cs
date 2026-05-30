using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualEventScheduler.Data.Models
{
    /// <summary>
    /// Join table linking Users to Events.
    /// A composite unique index on (EventId, UserId) prevents duplicate registrations.
    /// </summary>
    public class EventRegistration
    {
        [Key]
        public int Id { get; set; }

        /// <summary>The event this registration belongs to.</summary>
        [Required]
        public int EventId { get; set; }

        /// <summary>The user who registered.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Timestamp of when the user registered.</summary>
        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
