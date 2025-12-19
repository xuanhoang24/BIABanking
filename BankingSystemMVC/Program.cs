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
using BankingSystemMVC.Configuration;
using BankingSystemMVC.Infrastructure.Http;
using BankingSystemMVC.Services.Implementations.Accounts;
using BankingSystemMVC.Services.Implementations.Auth;
using BankingSystemMVC.Services.Implementations.Customers;
using BankingSystemMVC.Services.Implementations.Reports;
using BankingSystemMVC.Services.Interfaces.Accounts;
using BankingSystemMVC.Services.Interfaces.Auth;
using BankingSystemMVC.Services.Interfaces.Customers;
using BankingSystemMVC.Services.Interfaces.Reports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Customer API client
builder.Services.AddHttpClient("CustomerApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7098/");
})
.AddHttpMessageHandler<CustomerJwtHandler>();

// Admin API client
builder.Services.AddHttpClient("AdminApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7098/");
})
.AddHttpMessageHandler<AdminJwtHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CustomerJwtHandler>();
builder.Services.AddTransient<AdminJwtHandler>();

// JWT authentication for MVC authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPermissionAuthorization();

// App services
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();
builder.Services.AddScoped<IAccountApiClient, AccountApiClient>();
builder.Services.AddScoped<IAdminAuthApiClient, AdminAuthApiClient>();
builder.Services.AddScoped<IAdminAuditApiClient, AdminAuditApiClient>();
builder.Services.AddScoped<IAccountViewService, AccountViewService>();
builder.Services.AddScoped<ITransactionApiClient, TransactionApiClient>();
builder.Services.AddScoped<IAdminKycApiClient, AdminKycApiClient>();
builder.Services.AddScoped<IAdminUserApiClient, AdminUserApiClient>();
builder.Services.AddScoped<IReportApiClient, ReportApiClient>();
builder.Services.AddScoped<IAdminDashboardApiClient, AdminDashboardApiClient>();
builder.Services.AddScoped<IAdminCustomerApiClient, AdminCustomerApiClient>();
builder.Services.AddScoped<IAdminReportApiClient, AdminReportApiClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
