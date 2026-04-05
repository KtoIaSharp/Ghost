using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class CreateDealRequest
{
    [Required]
    public int TaskId { get; set; }
}

public class DealResponse
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string ExecutorNick { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CryptoPaymentUrl { get; set; }
}