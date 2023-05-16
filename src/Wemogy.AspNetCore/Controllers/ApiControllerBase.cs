using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Wemogy.AspNetCore.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        public Func<string> GetUserId { get; set; }

        public ApiControllerBase()
        {
            GetUserId = () => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
