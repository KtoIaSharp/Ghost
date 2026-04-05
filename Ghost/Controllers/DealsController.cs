using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ghost.Data;
using Ghost.DTOs;
using Ghost.Models;
using Ghost.Services;

namespace Ghost.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DealsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CryptoCloudService _cryptoCloud;
    
    public DealsController(AppDbContext db, CryptoCloudService cryptoCloud)
    {
        _db = db;
        _cryptoCloud = cryptoCloud;
    }
    
    private string GetCurrentUserHash()
    {
        return User.FindFirst("PhraseHash")?.Value ?? string.Empty;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDeal([FromBody] CreateDealRequest request)
    {
        var task = await _db.Tasks.FindAsync(request.TaskId);
        if (task == null || task.Status != "Open")
            return BadRequest("Задание недоступно");
        
        if (!task.IsPaid)
            return BadRequest("Бесплатное задание, сделка не требуется");
        
        var userHash = GetCurrentUserHash();
        
        // Проверяем, что пользователь не создатель задания
        if (task.CreatorPhraseHash == userHash)
            return BadRequest("Нельзя взять своё задание");
        
        var commission = task.RewardAmount * 0.05m;
        var totalAmount = task.RewardAmount;
        
        var deal = new Deal
        {
            TaskId = request.TaskId,
            CustomerPhraseHash = task.CreatorPhraseHash,
            ExecutorPhraseHash = userHash,
            Amount = totalAmount,
            Commission = commission,
            Status = "pending",
            AutoConfirmAt = DateTime.UtcNow.AddHours(24)
        };
        
        _db.Deals.Add(deal);
        
        // Обновляем задание
        task.Status = "InProgress";
        task.ExecutorPhraseHash = userHash;
        
        await _db.SaveChangesAsync();
        
        // Создаём счёт в CryptoCloud
        var paymentUrl = await _cryptoCloud.CreateInvoiceAsync(totalAmount, $"deal_{deal.Id}");
        
        return Ok(new DealResponse
        {
            Id = deal.Id,
            TaskId = deal.TaskId,
            ExecutorNick = task.ExecutorNick ?? "Исполнитель",
            Amount = deal.Amount,
            Commission = deal.Commission,
            Status = deal.Status,
            CryptoPaymentUrl = paymentUrl
        });
    }
    
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmDeal(int id)
    {
        var deal = await _db.Deals.FindAsync(id);
        if (deal == null)
            return NotFound();
        
        var userHash = GetCurrentUserHash();
        
        // Только заказчик может подтвердить
        if (deal.CustomerPhraseHash != userHash)
            return BadRequest("Только заказчик может подтвердить");
        
        deal.Status = "completed";
        deal.CompletedAt = DateTime.UtcNow;
        
        // Обновляем задание
        var task = await _db.Tasks.FindAsync(deal.TaskId);
        if (task != null)
        {
            task.Status = "Completed";
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Сделка подтверждена" });
    }
    
    [HttpPost("{id}/dispute")]
    public async Task<IActionResult> CreateDispute(int id, [FromBody] string reason)
    {
        var deal = await _db.Deals.FindAsync(id);
        if (deal == null)
            return NotFound();
        
        var userHash = GetCurrentUserHash();
        
        var dispute = new Dispute
        {
            DealId = id,
            OpenedBy = userHash,
            Reason = reason,
            Status = "open"
        };
        
        _db.Disputes.Add(dispute);
        deal.Status = "disputed";
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Спор открыт, администратор разберётся" });
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> GetMyDeals()
    {
        var userHash = GetCurrentUserHash();
        
        var deals = await _db.Deals
            .Where(d => d.CustomerPhraseHash == userHash || d.ExecutorPhraseHash == userHash)
            .Include(d => d.SchoolTask)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
        
        return Ok(deals);
    }
}