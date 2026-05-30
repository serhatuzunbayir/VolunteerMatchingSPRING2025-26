using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.API.DTOs
{
    /// <summary>DTO for updating an existing event's details.</summary>
    public class EventUpdateDto
    {
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
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
    }
}
