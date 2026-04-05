using System.Security.Cryptography;
using System.Text;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Services;

public class AuthService
{
    private readonly ApiService _apiService;
    private readonly SettingsService _settings;

    public AuthService(ApiService apiService, SettingsService settings)
    {
        _apiService = apiService;
        _settings = settings;
    }

    public bool IsLoggedIn => _settings.IsLoggedIn;

    public string Nickname => _settings.Nickname;

    public bool IsAdmin => _settings.IsAdmin;

    public string PhraseHash
    {
        get
        {
            // Если уже хеш (64+ символа) — возвращаем как есть
            if (!string.IsNullOrEmpty(_settings.PhraseHash) && _settings.PhraseHash.Length > 60)
                return _settings.PhraseHash;
            // Иначе хешируем
            return ComputeHash(_settings.PhraseHash);
        }
    }

    public async Task<LoginResponse?> LoginAsync(string phrase)
    {
        var response = await _apiService.LoginAsync(phrase);
        if (response != null)
        {
            // Сохраняем саму фразу (не хеш), SettingsService хранит её
            _settings.PhraseHash = phrase;
        }
        return response;
    }

    public void Logout()
    {
        _settings.ClearAuth();
    }

    private string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
