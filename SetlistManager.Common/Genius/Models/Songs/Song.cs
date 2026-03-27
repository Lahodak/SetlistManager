using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Song
{
    [JsonPropertyName("embed_content")]
    public string EmbedContent { get; set; } = default!;
}