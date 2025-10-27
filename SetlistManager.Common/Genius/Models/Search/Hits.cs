using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Search;

public class Hits
{
    [JsonProperty("type")]
    public string Type { get; set; } = default!;
    [JsonProperty("result")]
    public Result Result { get; set; } = default!;
}
