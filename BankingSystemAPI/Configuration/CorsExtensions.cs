namespace BankingSystemAPI.Configuration
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins("http://localhost:7000", "http://localhost:5173", "http://web:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
