using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Search;

public class Hits
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
    [JsonPropertyName("result")]
    public Result Result { get; set; } = default!;
}
