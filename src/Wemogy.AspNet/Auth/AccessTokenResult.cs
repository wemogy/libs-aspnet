using System.Text.Json.Serialization;

namespace Wemogy.AspNet.Auth
{
    public class AccessTokenResult
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("ext_expires_in")]
        public int ExtExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        public AccessTokenResult()
        {
            TokenType = string.Empty;
            Scope = string.Empty;
            AccessToken = string.Empty;
        }
    }
}
