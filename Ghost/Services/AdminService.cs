using Microsoft.EntityFrameworkCore;
using Ghost.Data;
using Ghost.Models;

namespace Ghost.Services;

public class AdminService
{
    private readonly AppDbContext _db;
    
    public AdminService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<bool> BanUserAsync(string phraseHash)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.PhraseHash == phraseHash);
        if (user == null) return false;
        
        user.IsBanned = true;
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _db.Tasks.FindAsync(taskId);
        if (task == null) return false;
        
        task.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }
    
    public async Task<List<SchoolTask>> GetAllTasksAsync()
    {
        return await _db.Tasks.ToListAsync();
    }
    
    public async Task<List<Complaint>> GetPendingComplaintsAsync()
    {
        return await _db.Complaints
            .Where(c => c.Status == "pending")
            .ToListAsync();
    }
    
    public async Task ResolveComplaintAsync(int complaintId, bool resolveInFavor)
    {
        var complaint = await _db.Complaints.FindAsync(complaintId);
        if (complaint == null) return;
        
        complaint.Status = resolveInFavor ? "resolved" : "rejected";
        
        if (resolveInFavor)
        {
            await BanUserAsync(complaint.OnPhraseHash);
        }
        
        await _db.SaveChangesAsync();
    }
}