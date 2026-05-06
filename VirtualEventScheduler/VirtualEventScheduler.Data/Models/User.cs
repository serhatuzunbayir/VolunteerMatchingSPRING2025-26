using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Attendee";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<EventRegistration> Registrations { get; set; }
    }
}
