using System.Text.Json.Serialization;

namespace mordor_api.Controllers
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }
    }
}
