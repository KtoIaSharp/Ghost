using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ghost.Data;
using Ghost.Services;
using Ghost.Middleware;
using System.Text;
using System.IO;

// Helper: конвертирует postgres:// URL в формат Npgsql
string? ConvertPostgresUrlToConnectionString(string? postgresUrl)
{
    if (string.IsNullOrEmpty(postgresUrl)) return null;
    
    // Если уже в формате "Host=..." — возвращаем как есть
    if (postgresUrl.StartsWith("Host=", StringComparison.OrdinalIgnoreCase))
        return postgresUrl;
    
    // Формат: postgres://user:password@host:port/dbname
    try
    {
        var uri = new Uri(postgresUrl);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.IsDefaultPort ? 5432 : uri.Port;
        var dbName = uri.LocalPath.TrimStart('/');
        
        return $"Host={host};Port={port};Database={dbName};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch
    {
        return null;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Конфигурация — единый fallback ключ
var jwtKey = builder.Configuration["Jwt:Key"] ?? "GhostSuperSecretKeyForJWT2024!LongEnough32Chars";
var key = Encoding.UTF8.GetBytes(jwtKey);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// НАСТРОЙКА SWAGGER С АВТОРИЗАЦИЕЙ
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен: Bearer {ваш_токен}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// База данных — автовыбор: PostgreSQL для продакшена, SQLite для разработки
var dbProvider = builder.Configuration["Db:Provider"] ?? "sqlite";

if (dbProvider == "postgres")
{
    // Пробуем несколько способов получения строки подключения
    var rawConnStr = builder.Configuration.GetConnectionString("Postgres");
    
    if (string.IsNullOrEmpty(rawConnStr))
        rawConnStr = Environment.GetEnvironmentVariable("DB_PostgresConnection");
    
    if (string.IsNullOrEmpty(rawConnStr))
        rawConnStr = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    if (string.IsNullOrEmpty(rawConnStr))
        rawConnStr = builder.Configuration["DB_PostgresConnection"];

    // Конвертируем postgres:// URL в формат Npgsql
    var connStr = ConvertPostgresUrlToConnectionString(rawConnStr);

    if (string.IsNullOrEmpty(connStr))
    {
        // Fallback на SQLite если PostgreSQL недоступен
        Console.WriteLine("⚠️ DATABASE_URL не найден, используем SQLite!");
        dbProvider = "sqlite";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    }
    else
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connStr));
        Console.WriteLine($"📦 База данных: PostgreSQL ({connStr.Split(';')[0]})");
    }
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    
    Console.WriteLine("📦 База данных: SQLite");
}

// JWT аутентификация
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Регистрация сервисов
builder.Services.AddHttpClient<CryptoCloudService>();
builder.Services.AddScoped<CryptoCloudService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<NicknameGenerator>();

var app = builder.Build();

app.UseStaticFiles();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Мидлвары
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Регистрируем AdminAuthMiddleware
app.UseMiddleware<AdminAuthMiddleware>();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (dbProvider == "postgres")
    {
        // PostgreSQL — используем миграции
        Console.WriteLine("🔄 Применяем миграции PostgreSQL...");
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Миграции применены.");
    }
    else
    {
        // SQLite — создаём если нет
        dbContext.Database.EnsureCreated();
        Console.WriteLine("✅ База данных SQLite проверена/создана.");
    }
}

app.Run();
