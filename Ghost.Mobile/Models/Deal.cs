namespace Ghost.Mobile.Models;

public class Deal
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public SchoolTask? SchoolTask { get; set; }
    public string CustomerPhraseHash { get; set; } = string.Empty;
    public string ExecutorPhraseHash { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public string? CryptoCloudInvoiceId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ProofImageUrl { get; set; }
    public bool CustomerConfirmed { get; set; }
    public DateTime? AutoConfirmAt { get; set; }
    public string? CryptoPaymentUrl { get; set; }

    public string StatusEmoji => Status switch
    {
        "pending" => "⏳",
        "paid" => "💰",
        "completed" => "✅",
        "disputed" => "⚠️",
        "refunded" => "↩️",
        _ => "❓"
    };

    public string StatusText => Status switch
    {
        "pending" => "Ожидает оплаты",
        "paid" => "Оплачено",
        "completed" => "Завершено",
        "disputed" => "Спор",
        "refunded" => "Возврат",
        _ => Status
    };
}

public class CreateDealRequest
{
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
