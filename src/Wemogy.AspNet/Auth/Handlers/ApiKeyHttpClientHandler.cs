using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Wemogy.AspNet.Auth.Handlers
{
    public class ApiKeyHttpClientHandler : HttpClientHandler
    {
        private readonly string _apiKeyHeader = "api-key";
        private readonly string _apiKey;

        public ApiKeyHttpClientHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add api-key header
            request.Headers.Remove(_apiKeyHeader);
            request.Headers.Add(_apiKeyHeader, _apiKey);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
