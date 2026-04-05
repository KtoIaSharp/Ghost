using Microsoft.EntityFrameworkCore;
using Ghost.Models;

namespace Ghost.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<SchoolTask> Tasks { get; set; }
    public DbSet<Deal> Deals { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<Dispute> Disputes { get; set; }
    public DbSet<MonetizationPoll> MonetizationPolls { get; set; }
    public DbSet<PollVote> PollVotes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Индексы для быстрого поиска
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhraseHash)
            .IsUnique();
        
        modelBuilder.Entity<SchoolTask>()
            .HasIndex(t => t.Status);
        
        modelBuilder.Entity<SchoolTask>()
            .HasIndex(t => t.ExpiresAt);
        
        modelBuilder.Entity<Deal>()
            .HasIndex(d => d.Status);
        
        modelBuilder.Entity<PollVote>()
            .HasIndex(pv => new { pv.PollId, pv.UserPhraseHash })
            .IsUnique();
    }
}