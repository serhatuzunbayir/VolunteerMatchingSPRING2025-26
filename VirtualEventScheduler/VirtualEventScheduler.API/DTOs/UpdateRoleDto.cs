using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.API.DTOs
{
    public class UpdateRoleDto
    {
        [Required]
        [RegularExpression("^(Admin|Staff|Attendee)$", ErrorMessage = "Role must be Admin, Staff, or Attendee")]
        public string Role { get; set; } = string.Empty;
    }
}
