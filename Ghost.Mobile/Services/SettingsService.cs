namespace Ghost.Mobile.Services;

public class SettingsService
{
    private const string TokenKey = "auth_token";
    private const string NicknameKey = "user_nickname";
    private const string IsAdminKey = "user_is_admin";
    private const string PhraseHashKey = "user_phrase_hash";
    private const string ServerUrlKey = "server_url";

    public string? Token
    {
        get => Preferences.Default.Get(TokenKey, (string?)null);
        set => Preferences.Default.Set(TokenKey, value);
    }

    public string Nickname
    {
        get => Preferences.Default.Get(NicknameKey, string.Empty);
        set => Preferences.Default.Set(NicknameKey, value);
    }

    public bool IsAdmin
    {
        get => Preferences.Default.Get(IsAdminKey, false);
        set => Preferences.Default.Set(IsAdminKey, value);
    }

    public string PhraseHash
    {
        get => Preferences.Default.Get(PhraseHashKey, string.Empty);
        set => Preferences.Default.Set(PhraseHashKey, value);
    }

    public string ServerUrl
    {
        get => Preferences.Default.Get(ServerUrlKey, "http://10.0.2.2:5274");
        set => Preferences.Default.Set(ServerUrlKey, value);
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public void ClearAuth()
    {
        Token = null;
        Nickname = string.Empty;
        IsAdmin = false;
        PhraseHash = string.Empty;
    }

    public void SaveAuth(string token, string nickname, bool isAdmin, string phraseHash)
    {
        Token = token;
        Nickname = nickname;
        IsAdmin = isAdmin;
        // phraseHash может быть пустым от сервера — фраза уже сохранена при логине
        if (!string.IsNullOrEmpty(phraseHash))
            PhraseHash = phraseHash;
    }
}
