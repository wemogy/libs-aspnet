using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Wemogy.AspNet.Refit;
using Xunit;

namespace Wemogy.AspNet.Tests.Middlewares;

public class RefitExceptionHandlerMiddlewareTests
{
    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task AssertStatusCodeForErrorAsync(HttpStatusCode expectedHttpStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        var message = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
        var response = new HttpResponseMessage(expectedHttpStatusCode)
        {
            Content = new StringContent("Bar")
        };
        var apiException = await ApiException.Create(message, HttpMethod.Get, response, new RefitSettings());
        var next = new RequestDelegate(_ => throw apiException);
        var middleware = new RefitExceptionHandlerMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)expectedHttpStatusCode);
    }
}
