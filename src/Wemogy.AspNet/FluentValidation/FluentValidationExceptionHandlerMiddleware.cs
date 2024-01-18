using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Wemogy.AspNet.FluentValidation
{
    public class FluentValidationExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public FluentValidationExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException exception)
            {
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await response.WriteAsJsonAsync(exception.Errors);
            }

            // Catch OperationCanceledException, when the request is aborted
            catch (OperationCanceledException)
            {
                if (context.RequestAborted.IsCancellationRequested)
                {
                    return;
                }

                throw;
            }
        }
    }
}
