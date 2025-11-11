using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models;

public class CodeExchangeResponseModel
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = default!;
}