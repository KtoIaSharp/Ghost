namespace Ghost.Mobile.Models;

public class Complaint
{
    public int Id { get; set; }
    public int? TaskId { get; set; }
    public string OnPhraseHash { get; set; } = string.Empty;
    public string FromPhraseHash { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateComplaintRequest
{
    public int? TaskId { get; set; }
    public string OnPhraseHash { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
