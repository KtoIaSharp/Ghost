using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class Complaint
{
    [Key]
    public int Id { get; set; }
    
    public int? TaskId { get; set; }
    
    [Required]
    public string OnPhraseHash { get; set; } = string.Empty;
    
    [Required]
    public string FromPhraseHash { get; set; } = string.Empty;
    
    public string Reason { get; set; } = string.Empty;
    
    public string Status { get; set; } = "pending";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}