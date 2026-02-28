using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Meta
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}