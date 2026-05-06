using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VirtualEventScheduler.Web.Models;

namespace VirtualEventScheduler.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("EventAPI");
            _httpContextAccessor = httpContextAccessor;

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterViewModel model)
        {
            var registerDto = new
            {
                fullName = model.FullName,
                email = model.Email,
                password = model.Password
            };

            var json = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Registration failed: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponseDto>(result);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginViewModel model)
        {
            var loginDto = new
            {
                email = model.Email,
                password = model.Password
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponseDto>(result);
        }

        public async Task<List<EventViewModel>> GetEventsAsync(DateTime? startDate = null, 
            DateTime? endDate = null, string status = null)
        {
            var query = "api/events?";
            
            if (startDate.HasValue)
                query += $"startDate={startDate.Value:yyyy-MM-dd}&";
            
            if (endDate.HasValue)
                query += $"endDate={endDate.Value:yyyy-MM-dd}&";
            
            if (!string.IsNullOrEmpty(status))
                query += $"status={status}&";

            var response = await _httpClient.GetAsync(query.TrimEnd('&'));
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<EventViewModel>>(result);
        }

        public async Task<EventViewModel> GetEventByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/events/{id}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EventViewModel>(result);
        }

        public async Task RegisterForEventAsync(int eventId)
        {
            EnsureAuthHeader();

            var response = await _httpClient.PostAsync($"api/events/{eventId}/registrations", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(ExtractMessage(error) ?? "Registration failed");
            }
        }

        public async Task<EventViewModel> CreateEventAsync(EventCreateViewModel model)
        {
            EnsureAuthHeader();

            var dto = new
            {
                title = model.Title,
                description = model.Description,
                dateTime = model.DateTime,
                location = model.Location,
                capacity = model.Capacity
            };

            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/events", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(ExtractMessage(error) ?? $"Create event failed ({(int)response.StatusCode})");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EventViewModel>(result);
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            EnsureAuthHeader();

            var response = await _httpClient.GetAsync("api/auth/users");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(ExtractMessage(error) ?? $"Failed to load users ({(int)response.StatusCode})");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<UserViewModel>>(result);
        }

        public async Task<UserViewModel> UpdateUserRoleAsync(int userId, string role)
        {
            EnsureAuthHeader();

            var dto = new { role };
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/auth/users/{userId}/role", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(ExtractMessage(error) ?? $"Failed to update role ({(int)response.StatusCode})");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserViewModel>(result);
        }

        private void EnsureAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                throw new Exception("Not authenticated");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private static string? ExtractMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;
            try
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                if (obj != null && obj.TryGetValue("message", out var msg) && msg != null)
                    return msg.ToString();
            }
            catch { }
            return body;
        }
    }
}
