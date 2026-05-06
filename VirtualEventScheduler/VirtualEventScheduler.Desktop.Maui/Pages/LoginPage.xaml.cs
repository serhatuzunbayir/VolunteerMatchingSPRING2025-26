using VirtualEventScheduler.Desktop.Maui.Models;
using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;

        public LoginPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Please enter email and password";
                lblError.IsVisible = true;
                return;
            }

            try
            {
                btnLogin.IsEnabled = false;
                btnLogin.Text = "Logging in...";
                lblError.IsVisible = false;

                var loginDto = new LoginDto
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text
                };

                var response = await _apiService.LoginAsync(loginDto);
                _apiService.SetToken(response.Token);

                if (response.Role != "Admin" && response.Role != "Staff")
                {
                    lblError.Text = "Only Admin and Staff can access this app";
                    lblError.IsVisible = true;
                    return;
                }

                await Navigation.PushAsync(new MainPage(_apiService, response));
            }
            catch (Exception ex)
            {
                lblError.Text = $"Login failed: {ex.Message}";
                lblError.IsVisible = true;
            }
            finally
            {
                btnLogin.IsEnabled = true;
                btnLogin.Text = "Login";
            }
        }
    }
}
