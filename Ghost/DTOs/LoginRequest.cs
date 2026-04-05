using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class LoginRequest
{
    [Required]
    public string Phrase { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}