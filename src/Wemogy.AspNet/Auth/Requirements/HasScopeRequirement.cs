using System;
using Microsoft.AspNet.Authorization;

namespace Wemogy.AspNet.Auth.Requirements
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
