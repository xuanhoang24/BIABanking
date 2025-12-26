using BankingSystemAPI.Configuration;
using BankingSystemAPI.Domain.Entities.Security;
using BankingSystemAPI.Infrastructure.Hubs;
using BankingSystemAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Options
builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordOptions")
);

builder.Services.Configure<BankingSystemAPI.Domain.Entities.Email.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

// Services
builder.Services.AddApplicationServices();
builder.Services.AddScoped<BankingSystemAPI.Application.Services.Interfaces.Email.IEmailService, BankingSystemAPI.Application.Services.Implementations.Email.EmailService>();

// CORS for SignalR
builder.Services.AddCorsPolicy();

// Rate Limiting
builder.Services.AddRateLimiting();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddPermissionAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

// Configure request size limits
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 10_485_760; // 10MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10_485_760; // 10MB
});

var app = builder.Build();

// Security Headers Middleware
app.Use(async (context, next) =>
{
    // Clickjacking protection
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Content-Security-Policy", "frame-ancestors 'none'");
    
    // Additional security headers
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // HSTS - HTTP Strict Transport Security
    app.UseHsts();
}

// Force HTTPS redirection
app.UseHttpsRedirection();

// Rate limiting middleware
app.UseRateLimiter();

app.UseCors("AllowMVC");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

await app.InitializeDatabaseAsync();

app.Run();
