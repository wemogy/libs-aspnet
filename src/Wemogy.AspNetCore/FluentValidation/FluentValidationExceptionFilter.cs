using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wemogy.AspNetCore.FluentValidation
{
    public class FluentValidationExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(validationException.Message);
            }
        }
    }
}
