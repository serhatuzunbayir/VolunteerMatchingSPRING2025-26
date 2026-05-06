namespace VirtualEventScheduler.Desktop.Maui.Models
{
    public class EventCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
    }
}
