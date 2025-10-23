using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Songs;

public class GetSongResponseModel
{
    [JsonProperty("meta")]
    public Meta Meta { get; set; } = default!;
    [JsonProperty("response")]
    public Response Response { get; set; } = default!;
}