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
                    "Ошибка входа",
                    "Не удалось войти. Возможные причины:\n\n" +
                    "• Сервер не запущен\n" +
                    "• Неверная кодовая фраза\n" +
                    "• Проблемы с сетью\n\n" +
                    $"Текущий URL: {_settings.ServerUrl}\n\n" +
                    "Проверьте что сервер запущен и URL правильный.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            if (errorMsg.Contains("Connection refused") || errorMsg.Contains("Failed to connect"))
            {
                errorMsg = $"Не удалось подключиться к серверу!\n\n" +
                          $"URL: {_settings.ServerUrl}\n\n" +
                          "Проверьте:\n" +
                          "• Бэкенд запущен (dotnet run)\n" +
                          "• URL в настройках правильный\n" +
                          "• Для эмулятора: http://10.0.2.2:5274\n" +
                          "• Для устройства: http://ВАШ_IP:5274";
            }
            
            await Application.Current.MainPage.DisplayAlert(
                "Ошибка подключения",
                errorMsg,
                "OK");
        }
    }
}
