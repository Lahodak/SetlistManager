using Newtonsoft.Json;


namespace SetlistManager.Common.Genius.Models;

public class CodeExchangeRequestModel
{
    private const string _responseType = "code";
    private const string _grantType = "authorization_code";

    [JsonProperty("code")]
    public string Code { get; set; } = default!;
    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; } = default!;    
    [JsonProperty("grant_type")]
    public string GrantType { get; set; } = _grantType;
    [JsonProperty("client_id")]
    public string ClientId { get; set; } = default!;
    [JsonProperty("redirect_uri")]
    public string RedirectUri { get; set; } = default!;
    [JsonProperty("response_type")]
    public string ResponseType { get; set; } = _responseType;
}