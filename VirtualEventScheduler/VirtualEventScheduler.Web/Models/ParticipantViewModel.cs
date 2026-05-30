namespace VirtualEventScheduler.Web.Models
{
    /// <summary>Represents a single event participant shown in the participants list.</summary>
    public class ParticipantViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
