using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class PollVote
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int PollId { get; set; }
    
    [Required]
    public string UserPhraseHash { get; set; } = string.Empty;
    
    public bool Vote { get; set; } // true = за, false = против
    
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}