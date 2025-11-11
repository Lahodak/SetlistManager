using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Songs;

public class Meta
{
    [JsonProperty("status")]
    public int Status { get; set; }
}