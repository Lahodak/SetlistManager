using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Songs;

public class GetSongResponseModel
{
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = default!;
    [JsonPropertyName("response")]
    public Response Response { get; set; } = default!;
}