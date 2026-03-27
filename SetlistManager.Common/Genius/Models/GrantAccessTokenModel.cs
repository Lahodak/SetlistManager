using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models;

public class GrantAccessTokenModel
{
    private const string _responseType = "code";
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = default!;
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = default!;
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = default!;
    [JsonPropertyName("state")]
    public string State { get; set; } = default!;
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = _responseType;
}