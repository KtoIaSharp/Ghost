using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ghost.Data;
using Ghost.DTOs;
using Ghost.Models;
using Ghost.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ghost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly HashService _hashService;
    private readonly NicknameGenerator _nicknameGenerator;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, HashService hashService, NicknameGenerator nicknameGenerator, IConfiguration config)
    {
        _db = db;
        _hashService = hashService;
        _nicknameGenerator = nicknameGenerator;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var phraseHash = _hashService.ComputeHash(request.Phrase);

        // Атомарная проверка: первый пользователь = админ
        // Используем транзакцию для предотвращения race condition
        using var transaction = await _db.Database.BeginTransactionAsync();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.PhraseHash == phraseHash);

        if (user == null)
        {
            // Новый пользователь
            var nickname = _nicknameGenerator.GenerateNickname();
            user = new User
            {
                PhraseHash = phraseHash,
                CurrentNick = nickname,
                CreatedAt = DateTime.UtcNow
            };

            // Атомарная проверка: если таблица пуста — делаем админом
            var userCount = await _db.Users.CountAsync();
            if (userCount == 0)
            {
                user.IsAdmin = true;
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        if (user.IsBanned)
        {
            return BadRequest(new { message = "Вы забанены" });
        }

        user.LastSeen = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            Nickname = user.CurrentNick,
            IsAdmin = user.IsAdmin
        });
    }

    private string GenerateJwtToken(User user)
    {
        // Используем тот же ключ, что и в Program.cs — без fallback
        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("PhraseHash", user.PhraseHash),
            new Claim("Nickname", user.CurrentNick),
            new Claim("IsAdmin", user.IsAdmin.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
