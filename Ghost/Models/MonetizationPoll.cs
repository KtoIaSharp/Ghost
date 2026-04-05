using System.ComponentModel.DataAnnotations;

namespace Ghost.Models;

public class MonetizationPoll
{
    [Key]
    public int Id { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndedAt { get; set; }
    
    public int VotesFor { get; set; } = 0;
    
    public int VotesAgainst { get; set; } = 0;
    
    public string Status { get; set; } = "active"; // active, completed
    
    public bool? Result { get; set; } // true = за монетизацию, false = против
    
    public bool MonetizationEnabled { get; set; } = false;
}