namespace SetlistManager.App.Options;

public class SetlistManagerApiOptions
{
    public const string SectionName = "SetlistManager.Api";
    public string ArtistsEndpoint { get; set; } = default!;
    public string AuthEndpoint { get; set; } = default!;
    public string UsersEndpoint { get; set; } = default!;
    public string SongsEndpoint { get; set; } = default!;
    public string SetlistsEndpoint { get; set; } = default!;
    public string LanguagesEndpoint { get; set; } = default!;
    public string InstrumentsEndpoint { get; set; } = default!;
    public string RoomsEndpoint { get; set; } = default!;
    public string TokensEndpoint { get; set; } = default!;
    public string RoomHubEndpoint { get; set; } = default!;
}