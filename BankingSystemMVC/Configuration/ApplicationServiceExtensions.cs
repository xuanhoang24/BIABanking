using BankingSystemMVC.Areas.Admin.Services.Implementations.Audit;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Auth;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Customers;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Dashboard;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Kyc;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Reports;
using BankingSystemMVC.Areas.Admin.Services.Implementations.Users;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Audit;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Auth;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Customers;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Dashboard;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Kyc;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Reports;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Users;
using BankingSystemMVC.Services.Implementations.Accounts;
using BankingSystemMVC.Services.Implementations.Auth;
using BankingSystemMVC.Services.Implementations.Customers;
using BankingSystemMVC.Services.Implementations.Reports;
using BankingSystemMVC.Services.Interfaces.Accounts;
using BankingSystemMVC.Services.Interfaces.Auth;
using BankingSystemMVC.Services.Interfaces.Customers;
using BankingSystemMVC.Services.Interfaces.Reports;

namespace BankingSystemMVC.Configuration
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Customer services
            services.AddScoped<IAuthApiClient, AuthApiClient>();
            services.AddScoped<ICustomerApiClient, CustomerApiClient>();
            services.AddScoped<IAccountApiClient, AccountApiClient>();
            services.AddScoped<IAccountViewService, AccountViewService>();
            services.AddScoped<ITransactionApiClient, TransactionApiClient>();
            services.AddScoped<IReportApiClient, ReportApiClient>();

            // Admin services
            services.AddScoped<IAdminAuthApiClient, AdminAuthApiClient>();
            services.AddScoped<IAdminAuditApiClient, AdminAuditApiClient>();
            services.AddScoped<IAdminKycApiClient, AdminKycApiClient>();
            services.AddScoped<IAdminUserApiClient, AdminUserApiClient>();
            services.AddScoped<IAdminDashboardApiClient, AdminDashboardApiClient>();
            services.AddScoped<IAdminCustomerApiClient, AdminCustomerApiClient>();
            services.AddScoped<IAdminReportApiClient, AdminReportApiClient>();

            return services;
        }
    }
}
