using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ghost.Data;
using Ghost.DTOs;
using Ghost.Models;
using System.Security.Claims;

namespace Ghost.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    
    public TasksController(AppDbContext db)
    {
        _db = db;
    }
    
    private string GetCurrentUserHash()
    {
        return User.FindFirst("PhraseHash")?.Value ?? string.Empty;
    }
    
    private string GetCurrentUserNick()
    {
        return User.FindFirst("Nickname")?.Value ?? string.Empty;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] string? category, [FromQuery] bool? onlyPaid)
    {
        var query = _db.Tasks
            .Where(t => t.Status == "Open" && t.ExpiresAt > DateTime.UtcNow)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);
        
        if (onlyPaid == true)
            query = query.Where(t => t.IsPaid);
        
        var tasks = await query
            .OrderByDescending(t => t.IsUrgent)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponse
            {
                Id = t.Id,
                CreatorNick = t.CreatorNick,
                Title = t.Title,
                Description = t.Description,
                RewardAmount = t.RewardAmount,
                Category = t.Category,
                IsPaid = t.IsPaid,
                IsUrgent = t.IsUrgent,
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt,
                Status = t.Status
            })
            .ToListAsync();
        
        return Ok(tasks);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();
        
        return Ok(task);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var userHash = GetCurrentUserHash();
        var userNick = GetCurrentUserNick();
        
        var task = new SchoolTask
        {
            CreatorPhraseHash = userHash,
            CreatorNick = userNick,
            Title = request.Title,
            Description = request.Description,
            RewardAmount = request.RewardAmount,
            Category = request.Category,
            IsPaid = request.IsPaid,
            IsUrgent = request.IsUrgent,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(72),
            Status = "Open"
        };
        
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        
        return Ok(task);
    }
    
    [HttpPost("{id}/take")]
    public async Task<IActionResult> TakeTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null || task.Status != "Open")
            return BadRequest("Задание недоступно");
        
        var userHash = GetCurrentUserHash();
        var userNick = GetCurrentUserNick();
        
        task.Status = "InProgress";
        task.ExecutorPhraseHash = userHash;
        task.ExecutorNick = userNick;
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Задание взято в работу" });
    }
    
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        var userHash = GetCurrentUserHash();
        
        if (task == null)
            return NotFound();
        
        if (task.ExecutorPhraseHash != userHash)
            return BadRequest("Не вы выполняете это задание");
        
        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        
        // Обновляем рейтинг исполнителя
        var executor = await _db.Users.FirstOrDefaultAsync(u => u.PhraseHash == userHash);
        if (executor != null)
        {
            executor.CompletedTasks++;
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Задание выполнено" });
    }
}