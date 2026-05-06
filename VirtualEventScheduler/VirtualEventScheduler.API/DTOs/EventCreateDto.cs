using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.API.DTOs
{
    public class EventCreateDto
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
        [Range(1, 10000)]
        public int Capacity { get; set; }
    }
}
