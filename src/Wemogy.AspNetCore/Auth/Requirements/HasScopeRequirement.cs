using System;
using Microsoft.AspNetCore.Authorization;

namespace Wemogy.AspNetCore.Auth.Requirements
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Scope { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scope">Scope to check</param>
        public HasScopeRequirement(string scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
