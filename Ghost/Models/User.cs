using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string PhraseHash { get; set; } = string.Empty;
    
    [Required]
    public string CurrentNick { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeen { get; set; }
    public double Rating { get; set; } = 0;
    public int CompletedTasks { get; set; } = 0;
    public bool IsBanned { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
}