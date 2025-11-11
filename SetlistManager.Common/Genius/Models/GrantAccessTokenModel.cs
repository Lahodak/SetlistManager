using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models;

public class GrantAccessTokenModel
{
    private const string _responseType = "code";
    [JsonProperty("client_id")]
    public string ClientId { get; set; } = default!;
    [JsonProperty("redirect_uri")]
    public string RedirectUri { get; set; } = default!;
    [JsonProperty("scope")]
    public string Scope { get; set; } = default!;
    [JsonProperty("state")]
    public string State { get; set; } = default!;
    [JsonProperty("response_type")]
    public string ResponseType { get; set; } = _responseType;
}