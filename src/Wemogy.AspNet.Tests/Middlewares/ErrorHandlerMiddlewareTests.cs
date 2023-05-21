using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Wemogy.AspNet.Middlewares;
using Wemogy.Core.Errors.Exceptions;
using Xunit;

namespace Wemogy.AspNet.Tests.Middlewares;

public class ErrorHandlerMiddlewareTests
{
    [Theory]
    [InlineData(
        typeof(AuthorizationErrorException),
        HttpStatusCode.Unauthorized)]
    [InlineData(
        typeof(ConflictErrorException),
        HttpStatusCode.Conflict)]
    [InlineData(
        typeof(FailureErrorException),
        HttpStatusCode.BadRequest)]
    [InlineData(
        typeof(NotFoundErrorException),
        HttpStatusCode.NotFound)]
    [InlineData(
        typeof(PreconditionFailedErrorException),
        HttpStatusCode.PreconditionFailed)]
    [InlineData(
        typeof(UnexpectedErrorException),
        HttpStatusCode.InternalServerError)]
    [InlineData(
        typeof(ValidationErrorException),
        HttpStatusCode.BadRequest)]
    public async Task AssertStatusCodeForErrorAsync(
        Type errorExceptionType,
        HttpStatusCode expectedHttpStatusCode)
    {
        // Arrange
        if (Activator.CreateInstance(errorExceptionType, "Test", "Test description", null) is not ErrorException errorException)
        {
            throw new Exception($"The type {errorExceptionType} is not a valid ErrorException");
        }

        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        var next = new RequestDelegate(_ => throw errorException);
        var middleware = new ErrorHandlerMiddleware(next);

        // Act
        await middleware.InvokeAsync(
            context);

        // Assert
        context.Response.StatusCode.Should().Be((int)expectedHttpStatusCode);
    }

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
        var middleware = new ErrorHandlerMiddleware(next);

        // Act
        await middleware.InvokeAsync(
            context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
