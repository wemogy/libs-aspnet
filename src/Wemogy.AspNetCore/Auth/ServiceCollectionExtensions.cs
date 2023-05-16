using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Wemogy.AspNetCore.Auth.Handlers;
using Wemogy.AspNetCore.Auth.Requirements;

namespace Wemogy.AspNetCore.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthorizationDefaultPolicy(this IServiceCollection services, params string[] authenticationSchemes)
        {
            services.AddAuthorizationCore(options =>
            {
                // Default [Authorize] Policy to a combination of both, Auth0 and Teams
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(authenticationSchemes);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            return services;
        }

        public static IServiceCollection AddScopeRequirements(
            this IServiceCollection services,
            string[] policyNames,
            string[] authenticationSchemes = null
        )
        {
            if (authenticationSchemes == null)
            {
                authenticationSchemes = new[] { JwtBearerDefaults.AuthenticationScheme };
            }

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddAuthorizationCore(options =>
            {
                foreach (string policyName in policyNames)
                {
                    options.AddPolicy(policyName, policy =>
                    {
                        foreach (var scheme in authenticationSchemes)
                        {
                            policy.AuthenticationSchemes.Add(scheme);
                        }

                        policy.Requirements.Add(new HasScopeRequirement(policyName));
                    });
                }
            });

            return services;
        }
    }
}
