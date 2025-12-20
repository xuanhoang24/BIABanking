using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Implementations.Customer;
using BankingSystemAPI.Application.Services.Implementations.Kyc;
using BankingSystemAPI.Application.Services.Implementations.Report;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Application.Services.Interfaces.Kyc;
using BankingSystemAPI.Application.Services.Interfaces.Report;
using BankingSystemAPI.Domain.Entities.Security;
using BankingSystemAPI.Extensions;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Persistence.Seed;
using BankingSystemAPI.Infrastructure.Security.Implements;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Options
builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordOptions")
);

// Services
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IKycAdminService, KycAdminService>();
builder.Services.AddScoped<ICustomerAdminService, CustomerAdminService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IKycService, KycService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IReportService, ReportService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            NameClaimType = JwtRegisteredClaimNames.Sub
        };
    });

builder.Services.AddPermissionAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    await RoleSeeder.SeedAsync(db);
    await PermissionSeeder.SeedAsync(db);
    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
