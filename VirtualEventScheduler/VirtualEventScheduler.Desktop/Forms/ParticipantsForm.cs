using VirtualEventScheduler.Desktop.Services;

namespace VirtualEventScheduler.Desktop.Forms
{
    /// <summary>
    /// Displays the list of participants registered for a selected event.
    /// Loads participant data from the API and shows it in a DataGridView.
    /// </summary>
    public partial class ParticipantsForm : Form
    {
        private readonly ApiService _apiService;

        // The ID and title of the event whose participants are being displayed
        private readonly int _eventId;
        private readonly string _eventTitle;

        public ParticipantsForm(ApiService apiService, int eventId, string eventTitle)
        {
            InitializeComponent();
            _apiService = apiService;
            _eventId = eventId;
            _eventTitle = eventTitle;

            this.Text = $"Participants - {eventTitle}";
            lblEventTitle.Text = $"Event: {eventTitle}";

            LoadParticipants();
        }

        /// <summary>Fetches and displays participants for the current event from the API.</summary>
        private async void LoadParticipants()
        {
            try
            {
                var participants = await _apiService.GetEventParticipantsAsync(_eventId);

                // Bind the result to the grid
                dgvParticipants.DataSource = participants;
                lblCount.Text = $"Total participants: {participants.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading participants: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadParticipants();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
