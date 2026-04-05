using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    private readonly SettingsService _settings;

    public LoginPage(LoginViewModel viewModel, SettingsService settings)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _settings = settings;

        // Показываем текущий URL сервера
        UpdateServerUrlDisplay();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateServerUrlDisplay();
    }

    private void UpdateServerUrlDisplay()
    {
        var url = _settings.ServerUrl;
        // Показываем URL на экране логина
        if (FindByName("ServerUrlLabel") is Label label)
        {
            label.Text = $"Сервер: {url}";
        }
    }
}
