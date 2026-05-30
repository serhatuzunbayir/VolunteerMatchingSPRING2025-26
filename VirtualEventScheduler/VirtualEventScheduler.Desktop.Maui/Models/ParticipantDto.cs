namespace VirtualEventScheduler.Desktop.Maui.Models
{
    /// <summary>Represents a participant entry returned from the API for a specific event.</summary>
    public class ParticipantDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
