using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    private readonly SettingsService _settings;

    public ProfilePage(ProfileViewModel viewModel, SettingsService settings)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _settings = settings;
        BindingContext = viewModel;

        NickLabel.Text = _settings.Nickname;
        ServerUrlLabel.Text = _settings.ServerUrl;

        ServerUrlButton.Clicked += async (s, e) =>
        {
            var result = await DisplayPromptAsync("URL сервера", "Введите адрес:", initialValue: _settings.ServerUrl);
            if (!string.IsNullOrWhiteSpace(result))
            {
                _settings.ServerUrl = result.TrimEnd('/');
                ServerUrlLabel.Text = _settings.ServerUrl;
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NickLabel.Text = _settings.Nickname;
    }
}
