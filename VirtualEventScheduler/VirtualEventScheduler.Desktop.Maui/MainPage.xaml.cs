using VirtualEventScheduler.Desktop.Maui.Models;
using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly LoginResponseDto _currentUser;

        public MainPage(ApiService apiService, LoginResponseDto currentUser)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentUser = currentUser;
            lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({_currentUser.Role})";
            LoadEvents();
        }

        private async void LoadEvents(string status = "Active")
        {
            try
            {
                var statusFilter = status == "All" ? null : status;
                var events = await _apiService.GetEventsAsync(statusFilter);
                eventsCollection.ItemsSource = events;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void btnCreate_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                await DisplayAlert("Validation", "Please enter title", "OK");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                await DisplayAlert("Validation", "Please enter valid capacity", "OK");
                return;
            }

            try
            {
                var eventDto = new EventCreateDto
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    DateTime = dtpDate.Date,
                    Location = txtLocation.Text,
                    Capacity = capacity
                };

                await _apiService.CreateEventAsync(eventDto);
                await DisplayAlert("Success", "Event created successfully!", "OK");

                txtTitle.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtLocation.Text = string.Empty;
                txtCapacity.Text = string.Empty;

                LoadEvents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void cmbStatus_Changed(object sender, EventArgs e)
        {
            var selected = cmbStatus.SelectedItem?.ToString() ?? "Active";
            LoadEvents(selected);
        }
    }
}
