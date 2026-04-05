using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly SettingsService _settings;

    public ApiService(SettingsService settings)
    {
        _settings = settings;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_settings.ServerUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    private void SetAuthHeader()
    {
        if (!string.IsNullOrEmpty(_settings.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);
        }
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default;
        }
    }

    private async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        try
        {
            SetAuthHeader();
            var json = data != null ? JsonConvert.SerializeObject(data) : "{}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return default;
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resultJson);
        }
        catch
        {
            return default;
        }
    }

    private async Task<bool> PostAsync(string url, object? data = null)
    {
        try
        {
            SetAuthHeader();
            var json = data != null ? JsonConvert.SerializeObject(data) : "{}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // ===== Auth =====
    public async Task<LoginResponse?> LoginAsync(string phrase)
    {
        var response = await PostAsync<LoginResponse>("/api/Auth/login", new { phrase });
        if (response != null)
        {
            _settings.SaveAuth(response.Token, response.Nickname, response.IsAdmin, "");
        }
        return response;
    }

    // ===== Tasks =====
    public async Task<List<SchoolTask>?> GetTasksAsync(string? category = null, bool? onlyPaid = null)
    {
        var url = "/api/Tasks";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={category}");
        if (onlyPaid == true) queryParams.Add("onlyPaid=true");
        if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);

        return await GetAsync<List<SchoolTask>>(url);
    }

    public async Task<SchoolTask?> GetTaskAsync(int id)
    {
        return await GetAsync<SchoolTask>($"/api/Tasks/{id}");
    }

    public async Task<bool> CreateTaskAsync(CreateTaskRequest request)
    {
        return await PostAsync("/api/Tasks", request);
    }

    public async Task<bool> TakeTaskAsync(int id)
    {
        return await PostAsync($"/api/Tasks/{id}/take");
    }

    public async Task<bool> CompleteTaskAsync(int id)
    {
        return await PostAsync($"/api/Tasks/{id}/complete");
    }

    // ===== Deals =====
    public async Task<DealResponse?> CreateDealAsync(int taskId)
    {
        return await PostAsync<DealResponse>("/api/Deals", new { TaskId = taskId });
    }

    public async Task<bool> ConfirmDealAsync(int dealId)
    {
        return await PostAsync($"/api/Deals/{dealId}/confirm");
    }

    public async Task<bool> DisputeDealAsync(int dealId, string reason)
    {
        return await PostAsync($"/api/Deals/{dealId}/dispute", reason);
    }

    public async Task<List<Deal>?> GetMyDealsAsync()
    {
        return await GetAsync<List<Deal>>("/api/Deals/my");
    }

    // ===== Chat Messages (polling-based) =====
    public async Task<List<ChatMessage>?> GetChatMessagesAsync(int dealId)
    {
        // Пока используем заглушку — endpoint ещё не реализован на бэкенде
        // В будущем: GET /api/Chat/{dealId}/messages
        return new List<ChatMessage>();
    }

    public async Task<bool> SendChatMessageAsync(int dealId, string text, string? imageUrl = null)
    {
        // Пока используем заглушку — endpoint ещё не реализован на бэкенде
        // В будущем: POST /api/Chat/{dealId}/messages
        return false;
    }

    // ===== Reviews =====
    public async Task<bool> CreateReviewAsync(CreateReviewRequest request)
    {
        // Пока заглушка — endpoint не реализован
        return false;
    }

    // ===== Complaints =====
    public async Task<bool> CreateComplaintAsync(CreateComplaintRequest request)
    {
        // Пока заглушка — endpoint не реализован
        return false;
    }

    // ===== Poll =====
    public async Task<object?> GetActivePollAsync()
    {
        return await GetAsync<object>("/api/Poll/active");
    }

    public async Task<bool> VoteAsync(int pollId, bool vote)
    {
        return await PostAsync($"/api/Poll/{pollId}/vote", new { Vote = vote });
    }
}
