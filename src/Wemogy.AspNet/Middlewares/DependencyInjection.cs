using Microsoft.AspNetCore.Builder;

namespace Wemogy.AspNet.Middlewares
{
    public static class DependencyInjection
    {
        public static void UseErrorHandlerMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
