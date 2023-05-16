using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Wemogy.AspNet.Auth.Auth0
{
    public static class ServiceCollectionExtensions
    {
        // Taken from: https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authority">Your Auth0 domain. Example: https://xxx.eu.auth0.com/</param>
        /// <param name="audience">Your Api Identifier. Example: https://api </param>
        /// <param name="roleClaimType"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAuth0Authentication(
            this AuthenticationBuilder builder,
            string authority,
            string audience,
            string roleClaimType = "")
        {
            return AddAuth0Authentication(builder, JwtBearerDefaults.AuthenticationScheme, authority, audience, roleClaimType);
        }

        /// <param name="builder"></param>
        /// <param name="scheme">Name of the Authentication Scheme</param>
        /// <param name="authority">Your Auth0 domain. Example: https://xxx.eu.auth0.com/</param>
        /// <param name="audience">Your Api Identifier. Example: https://api </param>
        /// <param name="roleClaimType"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAuth0Authentication(
            this AuthenticationBuilder builder,
            string scheme,
            string authority,
            string audience,
            string roleClaimType = "")
        {
            builder.AddJwtBearer(scheme, options =>
            {
                options.Authority = authority;
                options.Audience = audience;

                if (!string.IsNullOrEmpty(roleClaimType))
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = roleClaimType
                    };
                }

                // We have to hook the OnMessageReceived event in order to allow the JWT
                // authentication handler to read the access token from the query string when a
                // WebSocket or Server-Sent Events request comes in.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Read token from query string parameters, if provided there
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}
