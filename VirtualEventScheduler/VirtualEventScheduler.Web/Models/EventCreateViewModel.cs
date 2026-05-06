using System.ComponentModel.DataAnnotations;

namespace VirtualEventScheduler.Web.Models
{
    public class EventCreateViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public int Capacity { get; set; } = 10;
    }
}
