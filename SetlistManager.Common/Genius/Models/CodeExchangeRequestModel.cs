using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models;

public class CodeExchangeRequestModel
{
    private const string _responseType = "code";
    private const string _grantType = "authorization_code";

    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = default!;    
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = _grantType;
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = default!;
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = default!;
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = _responseType;
}