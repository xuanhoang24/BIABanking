using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystemMVC.Configuration
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // ADMIN PERMISSIONS
                AddAdminPermissionPolicy(options, PermissionCodes.DashboardView);
                AddAdminPermissionPolicy(options, PermissionCodes.CustomerRead);
                AddAdminPermissionPolicy(options, PermissionCodes.CustomerManage);
                AddAdminPermissionPolicy(options, PermissionCodes.KycRead);
                AddAdminPermissionPolicy(options, PermissionCodes.KycReview);
                AddAdminPermissionPolicy(options, PermissionCodes.TransactionRead);

                // CUSTOMER ACCESS
                options.AddPolicy(CustomerPolicies.Authenticated, policy =>
                {
                    policy.AuthenticationSchemes.Add("CustomerScheme");
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy(CustomerPolicies.KycRequired, policy =>
                {
                    policy.AuthenticationSchemes.Add("CustomerScheme"); 
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("kyc_verified", "true");
                });
            });

            return services;
        }

        private static void AddAdminPermissionPolicy(
            AuthorizationOptions options,
            string permission)
        {
            options.AddPolicy(permission, policy =>
            {
                policy.AuthenticationSchemes.Add("AdminScheme");
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("perm", permission);
            });
        }
    }
}
