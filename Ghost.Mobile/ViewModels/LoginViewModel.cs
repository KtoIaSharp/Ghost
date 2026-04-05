using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly SettingsService _settings;

    [ObservableProperty]
    private string _phrase = string.Empty;

    public LoginViewModel(AuthService authService, SettingsService settings)
    {
        _authService = authService;
        _settings = settings;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Phrase))
            return;

        // Сохраняем фразу ДО логина — она нужна для PhraseHash
        _settings.PhraseHash = Phrase.Trim();

        try
        {
            var result = await _authService.LoginAsync(Phrase.Trim());

            if (result != null)
            {
                await Shell.Current.GoToAsync("//main/tasks");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    "Не удалось войти. Проверьте кодовую фразу и URL сервера в настройках.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Ошибка подключения",
                $"Не удалось подключиться к серверу:\n{ex.Message}\n\nПроверьте что сервер запущен и URL в настройках правильный.",
                "OK");
        }
    }
}
