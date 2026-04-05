namespace Ghost.Mobile.Models;

public class User
{
    public int Id { get; set; }
    public string PhraseHash { get; set; } = string.Empty;
    public string CurrentNick { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeen { get; set; }
    public double Rating { get; set; }
    public int CompletedTasks { get; set; }
    public bool IsBanned { get; set; }
    public bool IsAdmin { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}

public class LoginRequest
{
    public string Phrase { get; set; } = string.Empty;
}
