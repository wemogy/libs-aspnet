using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Wemogy.AspNet.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        public Func<string> GetUserId { get; set; }

        public ApiControllerBase()
        {
            GetUserId = () =>
            {
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User is not authenticated");
            };
        }
    }
}
