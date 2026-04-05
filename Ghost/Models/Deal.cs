using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ghost.Models;

public class Deal
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TaskId { get; set; }

    [ForeignKey("TaskId")]
    public virtual SchoolTask? SchoolTask { get; set; }
    
    [Required]
    public string CustomerPhraseHash { get; set; } = string.Empty;
    
    [Required]
    public string ExecutorPhraseHash { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    
    public decimal Commission { get; set; }
    
    public string? CryptoCloudInvoiceId { get; set; }
    
    public string Status { get; set; } = "pending"; // pending, paid, completed, disputed, refunded
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public string? ProofImageUrl { get; set; }
    
    public bool CustomerConfirmed { get; set; } = false;
    
    public DateTime? AutoConfirmAt { get; set; } // Через 24 часа авто-подтверждение
    
}