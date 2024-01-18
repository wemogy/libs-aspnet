using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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

public class SystemExceptionHandlerMiddleware
{
    [Fact]
    public async Task CanceledRequestShouldBeIgnored()
    {
        // Arrange
        var abortedCts = new CancellationTokenSource();
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider(),
            RequestAborted = abortedCts.Token
        };
        var next = new RequestDelegate(
            c =>
            {
                // simulate that somewhere in the pipeline we are checking for cancellation
                c.RequestAborted.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            });
        var middleware = new ErrorExceptionHandlerMiddleware(next);

        // Act
        abortedCts.Cancel(); // simulate that the request was aborted
        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context));

        // Assert
        exception.Should().BeNull();
    }
}
