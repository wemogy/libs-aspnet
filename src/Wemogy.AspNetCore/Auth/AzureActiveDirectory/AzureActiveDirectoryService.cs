using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wemogy.AspNetCore.Auth.AzureActiveDirectory
{
    public class AzureActiveDirectoryService
    {
        private readonly HttpClient _client;
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scopePrefix;

        public AzureActiveDirectoryService(string instance, string tenantId, string clientId, string clientSecret, string scopePrefix = "")
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(instance);

            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scopePrefix = scopePrefix;
        }

        public async Task<AccessTokenResult> ExchangeAccessTokenAsync(string token, string[] scopes)
        {
            var scope = string.Join(" ", scopes.Select(x => $"{_scopePrefix}{x}"));

            var data = new[]
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("requested_token_use", "on_behalf_of"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("assertion", token)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_tenantId}/oauth2/v2.0/token");
            request.Content = new FormUrlEncodedContent(data);

            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonSerializer.Deserialize<AccessTokenResult>(json);
            return tokenResult;
        }
    }
}
