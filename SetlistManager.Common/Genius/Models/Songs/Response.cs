using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Response
{
    [JsonProperty("song")]
    public Song Song { get; set; } = default!;
}