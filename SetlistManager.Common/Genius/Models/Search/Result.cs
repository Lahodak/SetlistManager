using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Search;

public class Result
{
    [JsonProperty("api_path")]
    public string ApiPath { get; set; } = default!;
}