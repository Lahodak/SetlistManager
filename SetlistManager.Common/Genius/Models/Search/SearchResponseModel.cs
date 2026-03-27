using System.Text.Json.Serialization;

namespace SetlistManager.Common.Genius.Models.Search;

public class SearchResponseModel
{
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = default!;
    [JsonPropertyName("response")]
    public Response Response { get; set; } = default!;
}