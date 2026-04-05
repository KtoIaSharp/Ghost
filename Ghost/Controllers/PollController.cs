using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ghost.DTOs;
using Ghost.Services;
using System.Security.Claims;

namespace Ghost.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PollController : ControllerBase
{
    private readonly PollService _pollService;
    
    public PollController(PollService pollService)
    {
        _pollService = pollService;
    }
    
    private string GetCurrentUserHash()
    {
        return User.FindFirst("PhraseHash")?.Value ?? string.Empty;
    }
    
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePoll()
    {
        var poll = await _pollService.GetActivePollAsync();
        if (poll == null)
            return Ok(new { hasPoll = false });
        
        return Ok(new
        {
            hasPoll = true,
            pollId = poll.Id,
            votesFor = poll.VotesFor,
            votesAgainst = poll.VotesAgainst,
            status = poll.Status
        });
    }
    
    [HttpPost("{pollId}/vote")]
    public async Task<IActionResult> Vote(int pollId, [FromBody] VoteRequest request)
    {
        var userHash = GetCurrentUserHash();
        
        var result = await _pollService.VoteAsync(pollId, userHash, request.Vote);
        
        if (!result)
            return BadRequest(new { message = "Не удалось проголосовать" });
        
        return Ok(new { message = "Голос учтён" });
    }
}