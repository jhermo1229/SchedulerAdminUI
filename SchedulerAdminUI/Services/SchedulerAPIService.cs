using System.Net.Http;
using System.Net.Http.Json;
using SchedulerAdminUI.Models;

namespace SchedulerAdminUI.Services
{
    public class SchedulerApiService
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        public SchedulerApiService(string baseUrl = "http://localhost:5233/")
        {
            _baseUrl = NormalizeBaseUrl(baseUrl);
            _httpClient = CreateClient(_baseUrl);
        }

        public string BaseUrl => _baseUrl;

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = NormalizeBaseUrl(baseUrl);
            _httpClient = CreateClient(_baseUrl);
        }

        private static HttpClient CreateClient(string baseUrl)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        private static string NormalizeBaseUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "http://localhost:5233/";

            url = url.Trim();

            if (!url.EndsWith("/"))
                url += "/";

            return url;
        }

        public async Task<SchedulerStatusDto?> GetStatusAsync()
        {
            return await _httpClient.GetFromJsonAsync<SchedulerStatusDto>("api/scheduler/status");
        }

        public async Task<List<SchedulerJobDto>> GetJobsAsync()
        {
            var jobs = await _httpClient.GetFromJsonAsync<List<SchedulerJobDto>>("api/scheduler/jobs");
            return jobs ?? new List<SchedulerJobDto>();
        }

        public async Task<bool> RunJobNowAsync(string jobName)
        {
            var response = await _httpClient.PostAsync($"api/scheduler/jobs/{jobName}/run-now", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateJobEnabledAsync(string jobName, bool enabled)
        {
            var content = JsonContent.Create(new { enabled });
            var response = await _httpClient.PutAsync($"api/scheduler/jobs/{jobName}/enabled", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddRecipientAsync(string jobName, string email)
        {
            var content = JsonContent.Create(new { email });
            var response = await _httpClient.PostAsync($"api/scheduler/jobs/{jobName}/recipients", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveRecipientAsync(string jobName, string email)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/scheduler/jobs/{jobName}/recipients")
            {
                Content = JsonContent.Create(new { email })
            };

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateJobTimeAsync(string jobName, string timeOfDay)
        {
            var content = JsonContent.Create(new { timeOfDay });
            var response = await _httpClient.PutAsync($"api/scheduler/jobs/{jobName}/time", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateJobDaysAsync(string jobName, List<string> daysOfWeek)
        {
            var content = JsonContent.Create(new { daysOfWeek });
            var response = await _httpClient.PutAsync($"api/scheduler/jobs/{jobName}/days", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RenameJobAsync(string oldName, string newName)
        {
            var content = JsonContent.Create(new { newName });
            var response = await _httpClient.PutAsync($"api/scheduler/jobs/{oldName}/rename", content);
            return response.IsSuccessStatusCode;
        }
    }
}