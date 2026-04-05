using System.ComponentModel.DataAnnotations;

namespace Ghost.DTOs;

public class VoteRequest
{
    [Required]
    public bool Vote { get; set; }
}