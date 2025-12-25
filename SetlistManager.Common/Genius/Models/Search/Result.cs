using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Search;

public class Result
{
    [JsonProperty("api_path")]
    public string ApiPath { get; set; } = default!;
    [JsonProperty("url")]
    public string Url { get; set; } = default!;
    [JsonProperty("primary_artist_names")]
    public string PrimaryArtistNames { get; set; } = default!;
    [JsonProperty("title")]
    public string Title { get; set; } = default!;
    [JsonProperty("id")]
    public int Id { get; set; }
}