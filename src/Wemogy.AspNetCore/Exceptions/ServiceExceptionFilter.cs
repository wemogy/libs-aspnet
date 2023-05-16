using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wemogy.Exceptions.ServiceExceptions;

namespace Wemogy.AspNetCore.Exceptions
{
    public class ServiceExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Exception is ServiceException))
                return;

            context.ExceptionHandled = true;

            if (context.Exception is ItemNotFoundServiceException)
            {
                context.Result = new NotFoundObjectResult(context.Exception.Message);
                return;
            }

            if (context.Exception is ConflictServiceException)
            {
                context.Result = new ConflictObjectResult(context.Exception.Message);
                return;
            }

            if (context.Exception is ServiceUnavailableServiceException)
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = Convert.ToInt32(HttpStatusCode.ServiceUnavailable)
                };
                return;
            }

            if (context.Exception is InvalidArgumentServiceException)
            {
                context.Result = new BadRequestObjectResult(context.Exception.Message);
                return;
            }

            // Default
            context.Result = new BadRequestObjectResult(context.Exception.Message);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Has to be implemented by the interface
        }
    }
}
