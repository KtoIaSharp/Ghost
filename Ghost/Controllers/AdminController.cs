using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ghost.DTOs;
using Ghost.Services;

namespace Ghost.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;
    private readonly PollService _pollService;
    private readonly IConfiguration _config;

    public AdminController(AdminService adminService, PollService pollService, IConfiguration config)
    {
        _adminService = adminService;
        _pollService = pollService;
        _config = config;
    }

    private bool IsAdmin()
    {
        return User.FindFirst("IsAdmin")?.Value == "True";
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (!IsAdmin()) return Unauthorized();

        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("online")]
    public async Task<IActionResult> GetOnlineUsers()
    {
        if (!IsAdmin()) return Unauthorized();

        var users = await _adminService.GetOnlineUsersAsync();
        return Ok(users);
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> GetAllTasks()
    {
        if (!IsAdmin()) return Unauthorized();

        var tasks = await _adminService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("complaints")]
    public async Task<IActionResult> GetPendingComplaints()
    {
        if (!IsAdmin()) return Unauthorized();

        var complaints = await _adminService.GetPendingComplaintsAsync();
        return Ok(complaints);
    }

    [HttpPost("resolve-complaint")]
    public async Task<IActionResult> ResolveComplaint([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        if (request.TargetTaskId == null)
            return BadRequest("Не указан ID жалобы");

        await _adminService.ResolveComplaintAsync(request.TargetTaskId.Value, true);

        return Ok(new { message = "Жалоба рассмотрена" });
    }

    [HttpPost("ban")]
    public async Task<IActionResult> BanUser([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        if (string.IsNullOrEmpty(request.TargetPhraseHash))
            return BadRequest("Не указан phraseHash пользователя");

        var result = await _adminService.BanUserAsync(request.TargetPhraseHash);

        if (!result)
            return NotFound(new { message = "Пользователь не найден" });

        return Ok(new { message = "Пользователь забанен" });
    }

    [HttpPost("delete-task")]
    public async Task<IActionResult> DeleteTask([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        var result = await _adminService.DeleteTaskAsync(request.TargetTaskId ?? 0);

        if (!result)
            return NotFound(new { message = "Задание не найдено" });

        return Ok(new { message = "Задание удалено" });
    }

    [HttpPost("start-poll")]
    public async Task<IActionResult> StartPoll([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        await _pollService.StartPollAsync();

        return Ok(new { message = "Опрос запущен на 7 дней" });
    }

    [HttpPost("end-poll")]
    public async Task<IActionResult> EndPoll([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        var poll = await _pollService.GetActivePollAsync();
        if (poll == null)
            return BadRequest("Нет активного опроса");

        await _pollService.EndPollAsync(poll.Id);

        return Ok(new { message = "Опрос завершён" });
    }

    [HttpPost("enable-monetization")]
    public async Task<IActionResult> EnableMonetization([FromBody] AdminActionRequest request)
    {
        if (!IsAdmin()) return Unauthorized();

        if (request.AdminSecret != _config["Admin:SecretPhrase"])
            return BadRequest("Неверный секретный ключ");

        await _pollService.EnableMonetizationAsync();

        return Ok(new { message = "Монетизация включена" });
    }
}
