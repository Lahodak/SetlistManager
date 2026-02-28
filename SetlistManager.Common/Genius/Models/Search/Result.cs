
using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Search;

public class Result
{
    [JsonPropertyName("api_path")]
    public string ApiPath { get; set; } = default!;
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
    [JsonPropertyName("primary_artist_names")]
    public string PrimaryArtistNames { get; set; } = default!;
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;
    [JsonPropertyName("id")]
    public int Id { get; set; }
}