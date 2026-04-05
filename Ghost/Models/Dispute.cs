using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class Dispute
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int DealId { get; set; }
    
    [Required]
    public string OpenedBy { get; set; } = string.Empty;
    
    public string Reason { get; set; } = string.Empty;
    
    public string Status { get; set; } = "open";
    
    public string? ResolvedBy { get; set; }
    
    public string? Resolution { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ResolvedAt { get; set; }
}