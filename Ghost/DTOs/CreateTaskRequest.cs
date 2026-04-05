using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    public decimal RewardAmount { get; set; } = 0;
    
    public string Category { get; set; } = "Other";
    
    public bool IsPaid { get; set; } = false;
    
    public bool IsUrgent { get; set; } = false;
}

public class TaskResponse
{
    public int Id { get; set; }
    public string CreatorNick { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
}