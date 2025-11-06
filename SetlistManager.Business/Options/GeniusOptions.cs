namespace SetlistManager.Business.Options;

public class GeniusOptions
{
    public const string SectionName = "Genius";
    public string ApiBaseUrl { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;    
    public GetGrantAccessTokenRequestOptions GetGrantAccessTokenRequest { get; set; } = new();
}