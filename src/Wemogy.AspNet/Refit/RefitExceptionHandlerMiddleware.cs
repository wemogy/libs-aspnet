using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;

namespace Wemogy.AspNet.Refit
{
    public class RefitExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public RefitExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException exception)
            {
                context.Response.StatusCode = Convert.ToInt32(exception.StatusCode);
                context.Response.ContentType = "text/plain; charset=utf-8";
                if (exception.Content != null)
                {
                    await context.Response.WriteAsync(exception.Content);
                }
            }
        }
    }
}
