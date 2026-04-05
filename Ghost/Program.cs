using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ghost.Data;
using Ghost.Services;
using Ghost.Middleware;
using System.Text;
using System.IO;

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

// База данных (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    // Эта команда создаст БД и все таблицы, ТОЛЬКО если файла не существует
    dbContext.Database.EnsureCreated();
    Console.WriteLine("База данных проверена/создана.");
}

app.Run();
