using VirtualEventScheduler.Desktop.Maui.Models;
using VirtualEventScheduler.Desktop.Maui.Pages;
using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui
{
    /// <summary>
    /// Main dashboard for Admin and Staff users on macOS.
    /// Supports creating events, viewing/filtering the event list,
    /// viewing participant lists, and cancelling events.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private readonly ApiService _apiService;

        // Holds the current user's session info (role, name)
        private readonly LoginResponseDto _currentUser;

        public MainPage(ApiService apiService, LoginResponseDto currentUser)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentUser = currentUser;
            lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({_currentUser.Role})";
            LoadEvents();
        }

        /// <summary>
        /// Loads events from the API, optionally filtered by status.
        /// The filtering is handled server-side via LINQ in the API.
        /// </summary>
        private async void LoadEvents(string status = "Active")
        {
            try
            {
                // Pass null to get all statuses; otherwise pass the selected filter
                var statusFilter = status == "All" ? null : status;
                var events = await _apiService.GetEventsAsync(statusFilter);
                eventsCollection.ItemsSource = events;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load events: {ex.Message}", "OK");
            }
        }

        // ── Button handlers ──────────────────────────────────────────────────────

        /// <summary>Creates a new event using form field values.</summary>
        private async void btnCreate_Clicked(object sender, EventArgs e)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                await DisplayAlert("Validation", "Please enter a title", "OK");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                await DisplayAlert("Validation", "Please enter a valid capacity (positive number)", "OK");
                return;
            }

            // Combine DatePicker date with TimePicker time
            var selectedDateTime = dtpDate.Date.Add(tpTime.Time);

            if (selectedDateTime <= DateTime.Now)
            {
                await DisplayAlert("Validation", "Event date must be in the future", "OK");
                return;
            }

            try
            {
                var eventDto = new EventCreateDto
                {
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text?.Trim() ?? string.Empty,
                    DateTime = selectedDateTime,
                    Location = txtLocation.Text?.Trim() ?? string.Empty,
                    Capacity = capacity
                };

                await _apiService.CreateEventAsync(eventDto);
                await DisplayAlert("Success", "Event created successfully!", "OK");

                // Clear input fields
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

        /// <summary>Manually refreshes the event list.</summary>
        private void btnRefresh_Clicked(object sender, EventArgs e)
        {
            var selected = cmbStatus.SelectedItem?.ToString() ?? "Active";
            LoadEvents(selected);
        }

        /// <summary>Filters the event list when the status picker changes.</summary>
        private void cmbStatus_Changed(object sender, EventArgs e)
        {
            var selected = cmbStatus.SelectedItem?.ToString() ?? "Active";
            LoadEvents(selected);
        }

        /// <summary>Opens the ParticipantsPage for the tapped event.</summary>
        private async void btnViewParticipants_Clicked(object sender, EventArgs e)
        {
            // The CommandParameter on the button is bound to the EventDto of that row
            if (sender is Button btn && btn.CommandParameter is EventDto selectedEvent)
            {
                await Navigation.PushAsync(new ParticipantsPage(_apiService, selectedEvent.Id, selectedEvent.Title));
            }
        }

        /// <summary>
        /// Cancels the tapped event after confirmation.
        /// The API fires the OnEventCancelled delegate to notify subscribers.
        /// </summary>
        private async void btnCancelEvent_Clicked(object sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not EventDto selectedEvent)
                return;

            if (selectedEvent.Status == "Cancelled")
            {
                await DisplayAlert("Info", "This event is already cancelled.", "OK");
                return;
            }

            bool confirm = await DisplayAlert(
                "Confirm Cancellation",
                $"Are you sure you want to cancel \"{selectedEvent.Title}\"?",
                "Yes, Cancel",
                "No");

            if (!confirm)
                return;

            try
            {
                await _apiService.CancelEventAsync(selectedEvent.Id);
                await DisplayAlert("Cancelled", $"\"{selectedEvent.Title}\" has been cancelled.", "OK");
                LoadEvents(cmbStatus.SelectedItem?.ToString() ?? "All");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /// <summary>Logs the user out and navigates back to the LoginPage.</summary>
        private async void btnLogout_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
