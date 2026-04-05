namespace Ghost.Mobile.Models;

public class Review
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string FromPhraseHash { get; set; } = string.Empty;
    public string ToPhraseHash { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string StarsDisplay => new string('⭐', Rating);
}

public class CreateReviewRequest
{
    public int TaskId { get; set; }
    public string ToPhraseHash { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
