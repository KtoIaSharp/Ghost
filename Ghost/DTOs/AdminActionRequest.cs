using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class AdminActionRequest
{
    [Required]
    public string AdminSecret { get; set; } = string.Empty;

    // Для удаления заданий (int ID)
    public int? TargetTaskId { get; set; }

    // Для бана пользователей (string phraseHash)
    public string? TargetPhraseHash { get; set; }

    public string? Action { get; set; } // ban, delete_task, start_poll, enable_monetization
}
