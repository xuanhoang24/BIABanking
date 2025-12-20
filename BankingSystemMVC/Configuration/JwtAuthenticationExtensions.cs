using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankingSystemMVC.Configuration
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "CustomerScheme";
                    options.DefaultChallengeScheme = "CustomerScheme";
                })
                .AddJwtBearer("CustomerScheme", options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token =
                                context.HttpContext.Request.Cookies["customer_access_token"];
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            if (!context.Request.Path.StartsWithSegments("/Admin"))
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Auth/Login");
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters =
                        BuildTokenValidationParams(configuration);
                })
                .AddJwtBearer("AdminScheme", options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token =
                                context.HttpContext.Request.Cookies["admin_access_token"];
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/Admin"))
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Admin/Auth/Login");
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters =
                        BuildTokenValidationParams(configuration);
                });

            return services;
        }

        private static TokenValidationParameters BuildTokenValidationParams(
            IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                ),

                NameClaimType = System.Security.Claims.ClaimTypes.Name
            };
        }
    }
}
