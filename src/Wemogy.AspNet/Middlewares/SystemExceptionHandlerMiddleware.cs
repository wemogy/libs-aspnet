using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Wemogy.Core.Errors.Exceptions;

namespace Wemogy.AspNet.Middlewares
{
    public class SystemExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public SystemExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                // Catch OperationCanceledException, when the request is aborted
                if (context.RequestAborted.IsCancellationRequested)
                {
                    return;
                }

                throw;
            }
        }
    }
}
