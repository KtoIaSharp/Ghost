using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class Review
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TaskId { get; set; }
    
    [Required]
    public string FromPhraseHash { get; set; } = string.Empty;
    
    [Required]
    public string ToPhraseHash { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [MaxLength(500)]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}