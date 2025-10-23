using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Song
{
    [JsonProperty("embed_content")]
    public string EmbedContent { get; set; } = default!;
}