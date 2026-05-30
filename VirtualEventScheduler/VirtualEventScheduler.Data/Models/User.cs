using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Data.Models
{
    /// <summary>
    /// Represents a platform user. Roles: Admin, Staff, Attendee.
    /// Passwords are never stored as plain text — only the BCrypt hash is persisted.
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Full display name of the user.</summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        /// <summary>Unique email address used for login. Enforced unique at DB level.</summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>BCrypt-hashed password. Never store or compare plain text here.</summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>Role controls access: "Admin", "Staff", or "Attendee".</summary>
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Attendee";

        /// <summary>UTC timestamp of account creation.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>Events this user has registered for (navigation property).</summary>
        public virtual ICollection<EventRegistration> Registrations { get; set; }
    }
}
