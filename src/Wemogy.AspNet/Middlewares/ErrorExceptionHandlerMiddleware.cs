using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Wemogy.Core.Errors.Exceptions;

namespace Wemogy.AspNet.Middlewares
{
    public class ErrorExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorException exception)
            {
                var response = context.Response;
                HttpStatusCode statusCode;
                var message = exception.Message;

                switch (exception)
                {
                    case AuthorizationErrorException:
                        statusCode = HttpStatusCode.Forbidden;
                        break;
                    case ConflictErrorException:
                        statusCode = HttpStatusCode.Conflict;
                        break;
                    case FailureErrorException:
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case NotFoundErrorException:
                        statusCode = HttpStatusCode.NotFound;
                        break;
                    case PreconditionFailedErrorException:
                        statusCode = HttpStatusCode.PreconditionFailed;
                        break;
                    case UnexpectedErrorException:
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                    case ValidationErrorException:
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }

                response.StatusCode = (int)statusCode;
                await response.WriteAsJsonAsync(message);
            }

            // FluentValidation support
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
