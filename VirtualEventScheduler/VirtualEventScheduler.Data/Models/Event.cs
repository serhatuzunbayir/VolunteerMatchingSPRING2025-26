using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Data.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<EventRegistration> Registrations { get; set; }
    }
}
