using BankingSystemAPI.Models.Users.Roles;

namespace BankingSystemAPI.Security
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PermissionCodes.DashboardView,
                    policy => policy.RequireClaim("perm", PermissionCodes.DashboardView)
                );

                options.AddPolicy(
                    PermissionCodes.CustomerRead,
                    policy => policy.RequireClaim("perm", PermissionCodes.CustomerRead)
                );

                options.AddPolicy(
                    PermissionCodes.CustomerManage,
                    policy => policy.RequireClaim("perm", PermissionCodes.CustomerManage)
                );

                options.AddPolicy(
                    PermissionCodes.KycRead,
                    policy => policy.RequireClaim("perm", PermissionCodes.KycRead)
                );

                options.AddPolicy(
                    PermissionCodes.KycReview,
                    policy => policy.RequireClaim("perm", PermissionCodes.KycReview)
                );
            });

            return services;
        }
    }
}
