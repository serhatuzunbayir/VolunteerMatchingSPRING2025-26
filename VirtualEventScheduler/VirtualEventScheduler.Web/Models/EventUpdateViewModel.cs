using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Web.Models
{
    /// <summary>View model used in the event edit form.</summary>
    public class EventUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Date & Time")]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }

        // Read-only info shown on the form
        public int RegisteredCount { get; set; }
    }
}
