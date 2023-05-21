using System;
using Microsoft.AspNetCore.Authorization;

namespace Wemogy.AspNet.Auth.Requirements
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Scope { get; }

        /// <param name="scope">Scope to check</param>
        public HasScopeRequirement(string scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
