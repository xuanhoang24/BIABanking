using BankingSystemAPI.Configuration;
using BankingSystemAPI.Domain.Entities.Security;
using BankingSystemAPI.Infrastructure.Hubs;
using BankingSystemAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Options
builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordOptions")
);

// Services
builder.Services.AddApplicationServices();

// CORS for SignalR
builder.Services.AddCorsPolicy();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddPermissionAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowMVC");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

await app.InitializeDatabaseAsync();

app.Run();
