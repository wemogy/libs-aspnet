using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Wemogy.AspNet.Auth.Handlers
{
    public class ApiKeyHttpClientHandler : HttpClientHandler
    {
        private readonly string apiKeyHeader = "api-key";
        private readonly string apiKey;

        public ApiKeyHttpClientHandler(string apiKey)
        {
            this.apiKey = apiKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add api-key header
            request.Headers.Remove(apiKeyHeader);
            request.Headers.Add(apiKeyHeader, apiKey);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
