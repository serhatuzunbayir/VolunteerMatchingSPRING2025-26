using VirtualEventScheduler.Desktop.Models;
using VirtualEventScheduler.Desktop.Services;

namespace VirtualEventScheduler.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        private readonly ApiService _apiService;

        public LoginForm()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || 
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter email and password", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";

                var loginDto = new LoginDto
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text
                };

                var response = await _apiService.LoginAsync(loginDto);
                _apiService.SetToken(response.Token);

                if (response.Role != "Admin" && response.Role != "Staff")
                {
                    MessageBox.Show("Only Admin and Staff can access desktop app", 
                        "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Welcome {response.FullName}!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var mainForm = new MainForm(_apiService, response);
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }
    }
}
