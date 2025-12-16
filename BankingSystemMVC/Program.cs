using BankingSystemMVC.Areas.Admin.Services.Implements;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using BankingSystemMVC.Configuration;
using BankingSystemMVC.Infrastructure.Http;
using BankingSystemMVC.Services.Implements;
using BankingSystemMVC.Services.Interfaces;

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
builder.Services.AddAuthorization();


// App services
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();
builder.Services.AddScoped<IAccountApiClient, AccountApiClient>();
builder.Services.AddScoped<IAdminAuthApiClient, AdminAuthApiClient>();
builder.Services.AddScoped<IAdminAuditApiClient, AdminAuditApiClient>();
builder.Services.AddScoped<IAccountViewService, AccountViewService>();
builder.Services.AddScoped<ITransactionApiClient, TransactionApiClient>();
builder.Services.AddScoped<IAdminKycApiClient, AdminKycApiClient>();

builder.Services.AddScoped<IAdminDashboardApiClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AdminApi");
    return new AdminDashboardApiClient(httpClient);
});

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
