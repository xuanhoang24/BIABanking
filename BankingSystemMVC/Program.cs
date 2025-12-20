using BankingSystemMVC.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// API clients
builder.Services.AddApiClients(builder.Configuration);

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPermissionAuthorization();

// Application services
builder.Services.AddApplicationServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
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
