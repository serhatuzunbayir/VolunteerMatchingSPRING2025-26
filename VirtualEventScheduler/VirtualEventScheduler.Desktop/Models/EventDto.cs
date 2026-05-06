namespace VirtualEventScheduler.Desktop.Models
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public int RegisteredCount { get; set; }
        public bool IsFull { get; set; }
    }
}
