using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class SubmitProofRequest
{
    [Required]
    public int DealId { get; set; }
    
    [Required]
    public string ProofImageUrl { get; set; } = string.Empty;
}