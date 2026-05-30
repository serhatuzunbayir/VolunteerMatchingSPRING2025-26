using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VirtualEventScheduler.Desktop.Maui.Models;

namespace VirtualEventScheduler.Desktop.Maui.Services
{
    /// <summary>
    /// HTTP client wrapper for the Virtual Event Scheduler API.
    /// Used by the MAUI (macOS) desktop app to communicate with the shared REST API.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Stored JWT token used for authorized requests
        private string _token;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5202/")
            };
        }

        /// <summary>Stores the JWT token and attaches it to all future requests.</summary>
        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>Authenticates the user and returns a JWT token plus role/name info.</summary>
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Invalid email or password");

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponseDto>(result);
        }

        /// <summary>
        /// Returns events filtered by optional status.
        /// Delegates LINQ-based filtering to the API.
        /// </summary>
        public async Task<List<EventDto>> GetEventsAsync(string status = null)
        {
            var query = "api/events?";
            if (!string.IsNullOrEmpty(status))
                query += $"status={status}";

            var response = await _httpClient.GetAsync(query.TrimEnd('?'));
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<EventDto>>(result);
        }

        /// <summary>Creates a new event. Requires Admin or Staff token.</summary>
        public async Task<EventDto> CreateEventAsync(EventCreateDto eventCreateDto)
        {
            var json = JsonConvert.SerializeObject(eventCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/events", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Create event failed: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EventDto>(result);
        }

        /// <summary>
        /// Returns all participants registered for the given event.
        /// Requires Admin or Staff token.
        /// </summary>
        public async Task<List<ParticipantDto>> GetEventParticipantsAsync(int eventId)
        {
            var response = await _httpClient.GetAsync($"api/events/{eventId}/registrations");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to load participants: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ParticipantDto>>(result);
        }

        /// <summary>
        /// Cancels the specified event via the API.
        /// The API fires the OnEventCancelled delegate to notify subscribed handlers.
        /// Requires Admin or Staff token.
        /// </summary>
        public async Task CancelEventAsync(int eventId)
        {
            var response = await _httpClient.PutAsync($"api/events/{eventId}/cancel", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Cancel failed: {error}");
            }
        }
    }
}
