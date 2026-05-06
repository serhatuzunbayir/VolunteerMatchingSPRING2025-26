using VirtualEventScheduler.Desktop.Models;
using VirtualEventScheduler.Desktop.Services;

namespace VirtualEventScheduler.Desktop.Forms
{
    public partial class EventManagementForm : Form
    {
        private readonly ApiService _apiService;
        private readonly LoginResponseDto _currentUser;

        public EventManagementForm(ApiService apiService, LoginResponseDto currentUser)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentUser = currentUser;

            InitializeForm();
            LoadEvents();
        }

        private void InitializeForm()
        {
            cmbStatus.Items.AddRange(new object[] { "All", "Active", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;

            dgvEvents.AutoGenerateColumns = false;
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Width = 150 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Date", Width = 120 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Location", HeaderText = "Location", Width = 100 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RegisteredCount", HeaderText = "Registered", Width = 80 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Capacity", HeaderText = "Capacity", Width = 80 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 80 });
        }

        private async void LoadEvents(string status = null)
        {
            try
            {
                dgvEvents.DataSource = null;
                var statusFilter = status == "All" ? null : status;
                var events = await _apiService.GetEventsAsync(status: statusFilter);
                dgvEvents.DataSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error");
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter title", "Validation");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Please enter valid capacity", "Validation");
                return;
            }

            try
            {
                var eventDto = new EventCreateDto
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    DateTime = dtpDateTime.Value,
                    Location = txtLocation.Text,
                    Capacity = capacity
                };

                await _apiService.CreateEventAsync(eventDto);
                MessageBox.Show("Event created successfully!", "Success");

                txtTitle.Clear();
                txtDescription.Clear();
                txtLocation.Clear();
                txtCapacity.Clear();

                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating event: {ex.Message}", "Error");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEvents();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadEvents(cmbStatus.SelectedItem?.ToString());
        }
    }
}
