using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Wemogy.AspNet.Auth.AzureActiveDirectory
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddAzureActiveDirectoryAuthentication(
            this AuthenticationBuilder builder,
            string schema,
            string instance,
            string tenantId,
            string audience
        )
        {
            builder.AddJwtBearer(schema, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // Issuer in a multi-domain registration is the end-user's tenant,
                    // so we can't know which tenant issues the token and we can't validate it
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidAudiences = new[]
                    {
                        audience
                    }
                };

                options.MetadataAddress = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";
            });
            return builder;
        }
    }
}
