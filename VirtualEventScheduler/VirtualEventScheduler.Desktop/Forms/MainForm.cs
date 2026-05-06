using VirtualEventScheduler.Desktop.Models;
using VirtualEventScheduler.Desktop.Services;

namespace VirtualEventScheduler.Desktop.Forms
{
    public partial class MainForm : Form
    {
        private readonly ApiService _apiService;
        private readonly LoginResponseDto _currentUser;

        public MainForm(ApiService apiService, LoginResponseDto currentUser)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentUser = currentUser;

            lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({_currentUser.Role})";
        }

        private void btnEventManagement_Click(object sender, EventArgs e)
        {
            var eventForm = new EventManagementForm(_apiService, _currentUser);
            eventForm.ShowDialog();
        }

        private void btnViewEvents_Click(object sender, EventArgs e)
        {
            var eventForm = new EventManagementForm(_apiService, _currentUser);
            eventForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            var loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
