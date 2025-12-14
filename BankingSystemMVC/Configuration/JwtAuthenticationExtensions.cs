using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                    // Default is user side
                    options.DefaultAuthenticateScheme = "UserScheme";
                    options.DefaultChallengeScheme = "UserScheme";
                })
                .AddJwtBearer("UserScheme", options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.HttpContext.Request.Cookies["user_access_token"];
                            if (!string.IsNullOrEmpty(token))
                                context.Token = token;

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            // Only redirect if not in admin area and not already on login page
                            if (!context.Request.Path.StartsWithSegments("/Admin") && 
                                !context.Request.Path.StartsWithSegments("/Auth/Login"))
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Auth/Login");
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = BuildTokenValidationParams(configuration);
                })
                .AddJwtBearer("AdminScheme", options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.HttpContext.Request.Cookies["admin_access_token"];
                            if (!string.IsNullOrEmpty(token))
                                context.Token = token;

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            // Only redirect if in admin area and not already on login page
                            if (context.Request.Path.StartsWithSegments("/Admin") && 
                                !context.Request.Path.StartsWithSegments("/Admin/AdminAuth/Login"))
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Admin/AdminAuth/Login");
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = BuildTokenValidationParams(configuration, isAdmin: true);
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add("AdminScheme");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("is_admin", "true");
                });
                
                // Default policy allows anonymous access
                options.FallbackPolicy = null;
            });

            return services;
        }

        private static TokenValidationParameters BuildTokenValidationParams(IConfiguration configuration, bool isAdmin = false)
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

                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = isAdmin ? ClaimTypes.Role : ClaimTypes.Role
            };
        }
    }
}