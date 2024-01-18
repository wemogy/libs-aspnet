using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Wemogy.AspNet.Middlewares;
using Xunit;

namespace Wemogy.AspNet.Tests.Middlewares;

public class FluentValidationExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task AssertStatusCodeForFluentValidationExceptionAsync()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        var next = new RequestDelegate(_ => throw new ValidationException(new List<ValidationFailure>()));
        var middleware = new ErrorExceptionHandlerMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
