using System.Net;
using Refit;
using Wemogy.Exceptions.ServiceExceptions;

namespace Wemogy.AspNetCore.Exceptions
{
    public static class ServiceExceptionExtensions
    {
        /// <summary>
        /// Transforms a Refit.ApiException into a native Service Exception
        /// </summary>
        /// <param name="exception">The Refit.ApiException</param>
        /// <returns>A ServiceException</returns>
        public static ServiceException ToServiceException(this ApiException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
                return new ItemNotFoundServiceException(exception.Message);

            return new ApiServiceException(exception.Message);
        }
    }
}
