using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class SchoolTask
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string CreatorPhraseHash { get; set; } = string.Empty;
    
    [Required]
    public string CreatorNick { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    public decimal RewardAmount { get; set; } = 0; // В USDT
    
    public string Category { get; set; } = "Other";
    
    public bool IsPaid { get; set; } = false;
    
    public bool IsUrgent { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(72);
    
    public string Status { get; set; } = "Open"; // Open, InProgress, Completed, Cancelled
    
    public string? ExecutorPhraseHash { get; set; }
    
    public string? ExecutorNick { get; set; }
    
    public DateTime? CompletedAt { get; set; }
}