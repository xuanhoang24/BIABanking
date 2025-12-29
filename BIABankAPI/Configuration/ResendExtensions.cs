using BankingSystemAPI.Domain.Entities.Email;
using Resend;

namespace BankingSystemAPI.Configuration;

public static class ResendExtensions
{
    public static IServiceCollection AddResendEmail(this IServiceCollection services)
    {
        services.Configure<EmailSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? string.Empty;
            options.FromEmail = Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL") ?? string.Empty;
            options.FromName = Environment.GetEnvironmentVariable("RESEND_FROM_NAME") ?? "BIABank";
        });

        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? string.Empty;
        });
        services.AddTransient<IResend, ResendClient>();

        return services;
    }
}
