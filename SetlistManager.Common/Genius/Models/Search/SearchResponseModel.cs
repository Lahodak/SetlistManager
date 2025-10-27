using Newtonsoft.Json;

namespace SetlistManager.Common.Genius.Models.Search;

public class SearchResponseModel
{
    [JsonProperty("meta")]
    public Meta Meta { get; set; } = default!;
    [JsonProperty("response")]
    public Response Response { get; set; } = default!;
}