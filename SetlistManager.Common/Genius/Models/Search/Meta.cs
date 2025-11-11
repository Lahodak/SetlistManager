using Newtonsoft.Json;
namespace SetlistManager.Common.Genius.Models.Search;

public class Meta
{
    [JsonProperty("status")]
    public int Status { get; set; }
}