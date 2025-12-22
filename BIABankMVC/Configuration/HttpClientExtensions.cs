using BankingSystemMVC.Infrastructure.Http;

namespace BankingSystemMVC.Configuration
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration configuration)
        {
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7098/";

            // Customer API client
            services.AddHttpClient("CustomerApi", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddHttpMessageHandler<CustomerJwtHandler>();

            // Admin API client
            services.AddHttpClient("AdminApi", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddHttpMessageHandler<AdminJwtHandler>();

            services.AddTransient<CustomerJwtHandler>();
            services.AddTransient<AdminJwtHandler>();

            return services;
        }
    }
}
