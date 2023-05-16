using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public class GraphQlUserContext: Dictionary<string, object>
    {
        public bool UserIsAuthenticated { get; private set; }

        public GraphQlUserContext(HttpContext httpContext)
        {
            this.UserIsAuthenticated = httpContext.User.Identity.IsAuthenticated;
        }
    }
}
