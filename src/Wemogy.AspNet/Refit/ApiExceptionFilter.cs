using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;

namespace Wemogy.AspNet.Refit
{
    public class ApiExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ApiException exception)
            {
                context.Result = new ObjectResult(exception.Content)
                {
                    StatusCode = Convert.ToInt32(exception.StatusCode),
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
