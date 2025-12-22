using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Implementations.Customer;
using BankingSystemAPI.Application.Services.Implementations.Email;
using BankingSystemAPI.Application.Services.Implementations.Kyc;
using BankingSystemAPI.Application.Services.Implementations.Report;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Application.Services.Interfaces.Email;
using BankingSystemAPI.Application.Services.Interfaces.Kyc;
using BankingSystemAPI.Application.Services.Interfaces.Report;
using BankingSystemAPI.Infrastructure.Security.Implements;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using BankingSystemAPI.Infrastructure.Services;

namespace BankingSystemAPI.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Admin service
            services.AddScoped<AuditService>();
            services.AddScoped<AdminUserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IKycAdminService, KycAdminService>();
            services.AddScoped<ICustomerAdminService, CustomerAdminService>();

            // Customer service
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IKycService, KycService>();

            // Security services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPermissionService, PermissionService>();

            // Report services
            services.AddScoped<IReportService, ReportService>();

            // Notification service
            services.AddScoped<NotificationService>();

            // Email service
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
