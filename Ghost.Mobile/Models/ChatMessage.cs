namespace Ghost.Mobile.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public string SenderPhraseHash { get; set; } = string.Empty;
    public string SenderNick { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsMine { get; set; }

    public string TimeDisplay => SentAt.ToString("HH:mm");
}

public class SendMessageRequest
{
    public int DealId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
