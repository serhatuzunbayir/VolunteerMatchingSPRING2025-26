using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VirtualEventScheduler.Desktop.Models;

namespace VirtualEventScheduler.Desktop.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5202/")
            };
        }

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponseDto>(result);
        }

        public async Task<List<EventDto>> GetEventsAsync(DateTime? startDate = null, 
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
            return JsonConvert.DeserializeObject<List<EventDto>>(result);
        }

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

        public async Task<List<ParticipantDto>> GetEventParticipantsAsync(int eventId)
        {
            var response = await _httpClient.GetAsync($"api/events/{eventId}/registrations");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ParticipantDto>>(result);
        }
    }

    public class ParticipantDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
