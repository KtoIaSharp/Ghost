namespace Ghost.Mobile.Models;

public class SchoolTask
{
    public int Id { get; set; }
    public string CreatorPhraseHash { get; set; } = string.Empty;
    public string CreatorNick { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; }
    public string Category { get; set; } = "Other";
    public bool IsPaid { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(72);
    public string Status { get; set; } = "Open";
    public string? ExecutorPhraseHash { get; set; }
    public string? ExecutorNick { get; set; }
    public DateTime? CompletedAt { get; set; }

    public string CategoryEmoji => Category switch
    {
        "Math" => "📐",
        "Physics" => "⚛️",
        "Chemistry" => "🧪",
        "Biology" => "🧬",
        "History" => "📜",
        "Literature" => "📚",
        "Programming" => "💻",
        "Language" => "🌍",
        "Art" => "🎨",
        "Music" => "🎵",
        "Sports" => "⚽",
        _ => "📝"
    };

    public string RewardDisplay => IsPaid ? $"{RewardAmount} USDT" : "Бесплатно";

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - CreatedAt;
            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч назад";
            return $"{(int)diff.TotalDays} дн назад";
        }
    }
}

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; }
    public string Category { get; set; } = "Other";
    public bool IsPaid { get; set; }
    public bool IsUrgent { get; set; }
}

public class TaskResponse
{
    public int Id { get; set; }
    public string CreatorNick { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
