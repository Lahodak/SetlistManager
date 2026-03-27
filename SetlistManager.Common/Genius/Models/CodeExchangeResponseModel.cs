using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models;

public class CodeExchangeResponseModel
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}