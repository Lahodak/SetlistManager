using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models;

public class GrantAccessTokenResultModel
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;
    [JsonPropertyName("state")]
    public string State { get; set; } = default!;
}