using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Response
{
    [JsonPropertyName("song")]
    public Song Song { get; set; } = default!;
}