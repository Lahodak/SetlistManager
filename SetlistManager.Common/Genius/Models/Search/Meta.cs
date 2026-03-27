using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Search;

public class Meta
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}