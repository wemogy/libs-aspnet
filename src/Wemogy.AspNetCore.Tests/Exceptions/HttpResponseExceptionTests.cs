using System.Net;
using Xunit;
using Wemogy.Exceptions.HttpExceptions;

namespace Wemogy.AspNetCore.Tests.Exceptions
{
    public class HttpResponseExceptionTests
    {
        [Fact]
        public void HttpResponseException_InitWithStatusCode_Works()
        {
            // Arrange
            var code = HttpStatusCode.NotFound; // 404
            var message = "Lorem ipsum";

            // Act
            var exception = new HttpResponseException(code, message);

            // Assert
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal(message, exception.Message);
        }
    }
}
