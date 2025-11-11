using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Search;

public class Response
{
    [JsonProperty("hits")]
    public List<Hits> Hits { get; set; } = default!;
}