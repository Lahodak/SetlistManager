using Newtonsoft.Json;
namespace SetlistManager.Common.Genius.Models;

public class GrantAccessTokenResultModel
{
    [JsonProperty("code")]
    public string Code { get; set; } = default!;
    [JsonProperty("state")]
    public string State { get; set; } = default!;
}