using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly SettingsService _settings;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private bool _isAdmin;

    [ObservableProperty]
    private int _completedTasks;

    [ObservableProperty]
    private double _rating;

    [ObservableProperty]
    private string _serverUrl = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasActivePoll;

    [ObservableProperty]
    private int _pollId;

    [ObservableProperty]
    private int _votesFor;

    [ObservableProperty]
    private int _votesAgainst;

    public ProfileViewModel(AuthService authService, SettingsService settings)
    {
        _authService = authService;
        _settings = settings;
    }

    public void LoadProfile()
    {
        Nickname = _settings.Nickname;
        IsAdmin = _settings.IsAdmin;
        ServerUrl = _settings.ServerUrl;
        CompletedTasks = 0; // Пока нет endpoint для получения профиля
        Rating = 0;
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        Shell.Current.GoToAsync("//login");
    }

    [RelayCommand]
    private async Task LoadPollAsync()
    {
        try
        {
            var apiService = new ApiService(_settings);
            var poll = await apiService.GetActivePollAsync();

            if (poll is System.Collections.Generic.Dictionary<string, object> dict && dict.ContainsKey("hasPoll") && (bool)dict["hasPoll"])
            {
                HasActivePoll = true;
                PollId = dict.ContainsKey("pollId") ? Convert.ToInt32(dict["pollId"]) : 0;
                VotesFor = dict.ContainsKey("votesFor") ? Convert.ToInt32(dict["votesFor"]) : 0;
                VotesAgainst = dict.ContainsKey("votesAgainst") ? Convert.ToInt32(dict["votesAgainst"]) : 0;
            }
            else
            {
                HasActivePoll = false;
            }
        }
        catch
        {
            HasActivePoll = false;
        }
    }

    [RelayCommand]
    private async Task VotePollAsync(bool vote)
    {
        try
        {
            var apiService = new ApiService(_settings);
            var result = await apiService.VoteAsync(PollId, vote);

            if (result)
            {
                StatusMessage = "Голос учтён!";
                await LoadPollAsync();
            }
            else
            {
                StatusMessage = "Не удалось проголосовать";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ChangeServerUrlAsync()
    {
        var result = await App.Current.MainPage.DisplayPromptAsync(
            "URL сервера", "Введите адрес сервера:",
            initialValue: _settings.ServerUrl);

        if (!string.IsNullOrWhiteSpace(result))
        {
            _settings.ServerUrl = result.TrimEnd('/');
            ServerUrl = _settings.ServerUrl;
            StatusMessage = "URL сервера обновлён";
        }
    }
}
