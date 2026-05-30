using VirtualEventScheduler.Desktop.Models;
using VirtualEventScheduler.Desktop.Services;

namespace VirtualEventScheduler.Desktop.Forms
{
    /// <summary>
    /// Provides event management functionality for Admin and Staff users:
    /// creating events, viewing and filtering the event list,
    /// viewing participant lists, and cancelling events.
    /// </summary>
    public partial class EventManagementForm : Form
    {
        private readonly ApiService _apiService;

        // Stores the currently logged-in user's session info (role, name, token)
        private readonly LoginResponseDto _currentUser;

        public EventManagementForm(ApiService apiService, LoginResponseDto currentUser)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentUser = currentUser;

            InitializeEventGrid();
            LoadEvents();
        }

        /// <summary>Configures the DataGridView columns for the event list.</summary>
        private void InitializeEventGrid()
        {
            // Status filter options
            cmbStatus.Items.AddRange(new object[] { "All", "Active", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;

            // Set up DataGridView columns
            dgvEvents.AutoGenerateColumns = false;
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Width = 150 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Date", Width = 120 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Location", HeaderText = "Location", Width = 100 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RegisteredCount", HeaderText = "Registered", Width = 80 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Capacity", HeaderText = "Capacity", Width = 80 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 80 });
        }

        /// <summary>
        /// Loads the event list from the API, optionally filtered by status.
        /// Uses LINQ-based filtering on the API side.
        /// </summary>
        private async void LoadEvents(string status = null)
        {
            try
            {
                dgvEvents.DataSource = null;

                // Pass null for "All" so the API returns unfiltered results
                var statusFilter = status == "All" ? null : status;
                var events = await _apiService.GetEventsAsync(status: statusFilter);
                dgvEvents.DataSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Returns the EventDto of the row currently selected in the grid, or null if none.</summary>
        private EventDto GetSelectedEvent()
        {
            if (dgvEvents.SelectedRows.Count == 0)
                return null;

            return dgvEvents.SelectedRows[0].DataBoundItem as EventDto;
        }

        // ── Button handlers ──────────────────────────────────────────────────────

        /// <summary>Creates a new event using the values entered in the form fields.</summary>
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            // Validate required input
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Please enter a valid capacity (positive number)", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpDateTime.Value <= DateTime.Now)
            {
                MessageBox.Show("Event date must be in the future", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var eventDto = new EventCreateDto
                {
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    DateTime = dtpDateTime.Value,
                    Location = txtLocation.Text.Trim(),
                    Capacity = capacity
                };

                await _apiService.CreateEventAsync(eventDto);
                MessageBox.Show("Event created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the form fields after successful creation
                txtTitle.Clear();
                txtDescription.Clear();
                txtLocation.Clear();
                txtCapacity.Clear();

                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating event: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Refreshes the event list from the API.</summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEvents();
        }

        /// <summary>Applies the status filter selected in the combo box.</summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadEvents(cmbStatus.SelectedItem?.ToString());
        }

        /// <summary>Opens the ParticipantsForm for the selected event row.</summary>
        private void btnViewParticipants_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedEvent();
            if (selected == null)
            {
                MessageBox.Show("Please select an event from the list first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Open the participants form for the selected event
            var form = new ParticipantsForm(_apiService, selected.Id, selected.Title);
            form.ShowDialog();
        }

        /// <summary>
        /// Cancels the selected event after confirmation.
        /// The API fires the OnEventCancelled delegate to notify subscribers.
        /// </summary>
        private async void btnCancelEvent_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedEvent();
            if (selected == null)
            {
                MessageBox.Show("Please select an event from the list first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selected.Status == "Cancelled")
            {
                MessageBox.Show("This event is already cancelled.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to cancel \"{selected.Title}\"?",
                "Confirm Cancellation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                await _apiService.CancelEventAsync(selected.Id);
                MessageBox.Show($"Event \"{selected.Title}\" has been cancelled.", "Cancelled",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling event: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
