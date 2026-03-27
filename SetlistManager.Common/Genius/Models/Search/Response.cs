using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Search;

public class Response
{
    [JsonPropertyName("hits")]
    public List<Hits> Hits { get; set; } = default!;
}