using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wemogy.AspNet.Auth.Requirements;

namespace Wemogy.AspNet.Auth.Handlers
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type.Contains("scope")))
            {
                return Task.CompletedTask;
            }

            // Split the scopes string into an array
            var scopes = context.User
                .FindFirst(c => c.Type.Contains("scope"))?.Value
                .Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes?.Any(s => s == requirement.Scope) == true)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
