using Microsoft.EntityFrameworkCore;
using Ghost.Data;
using Ghost.Models;

namespace Ghost.Services;

public class PollService
{
    private readonly AppDbContext _db;
    
    public PollService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<MonetizationPoll?> GetActivePollAsync()
    {
        return await _db.MonetizationPolls
            .FirstOrDefaultAsync(p => p.Status == "active");
    }
    
    public async Task StartPollAsync()
    {
        var activePoll = await GetActivePollAsync();
        if (activePoll != null) return;
        
        var poll = new MonetizationPoll
        {
            StartedAt = DateTime.UtcNow,
            Status = "active"
        };
        
        _db.MonetizationPolls.Add(poll);
        await _db.SaveChangesAsync();
    }
    
    public async Task<bool> VoteAsync(int pollId, string userPhraseHash, bool vote)
    {
        var existingVote = await _db.PollVotes
            .FirstOrDefaultAsync(v => v.PollId == pollId && v.UserPhraseHash == userPhraseHash);
        
        if (existingVote != null) return false;
        
        var poll = await _db.MonetizationPolls.FindAsync(pollId);
        if (poll == null || poll.Status != "active") return false;
        
        var voteEntity = new PollVote
        {
            PollId = pollId,
            UserPhraseHash = userPhraseHash,
            Vote = vote
        };
        
        _db.PollVotes.Add(voteEntity);
        
        if (vote)
            poll.VotesFor++;
        else
            poll.VotesAgainst++;
        
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> EndPollAsync(int pollId)
    {
        var poll = await _db.MonetizationPolls.FindAsync(pollId);
        if (poll == null || poll.Status != "active") return false;
        
        poll.Status = "completed";
        poll.EndedAt = DateTime.UtcNow;
        
        var totalVotes = poll.VotesFor + poll.VotesAgainst;
        if (totalVotes > 0)
        {
            poll.Result = poll.VotesFor > poll.VotesAgainst;
        }
        
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task EnableMonetizationAsync()
    {
        var activePoll = await GetActivePollAsync();
        if (activePoll != null)
        {
            activePoll.MonetizationEnabled = true;
            await _db.SaveChangesAsync();
        }
    }
}