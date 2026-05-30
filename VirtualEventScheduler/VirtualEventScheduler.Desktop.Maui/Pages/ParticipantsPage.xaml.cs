using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui.Pages
{
    /// <summary>
    /// Displays all participants registered for a specific event.
    /// Accessible only by Admin and Staff users.
    /// </summary>
    public partial class ParticipantsPage : ContentPage
    {
        private readonly ApiService _apiService;

        // The event whose participants are being shown
        private readonly int _eventId;
        private readonly string _eventTitle;

        public ParticipantsPage(ApiService apiService, int eventId, string eventTitle)
        {
            InitializeComponent();
            _apiService = apiService;
            _eventId = eventId;
            _eventTitle = eventTitle;

            lblEventTitle.Text = $"Participants: {eventTitle}";
            LoadParticipants();
        }

        /// <summary>Fetches and displays the participant list from the API.</summary>
        private async void LoadParticipants()
        {
            try
            {
                var participants = await _apiService.GetEventParticipantsAsync(_eventId);
                participantsCollection.ItemsSource = participants;
                lblCount.Text = $"Total registered: {participants.Count}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void btnRefresh_Clicked(object sender, EventArgs e)
        {
            LoadParticipants();
        }

        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
