namespace SetlistManager.App.Options;

public class GeniusOptions
{
    public const string SectionName = "Genius";
    public string BaseApiUrl { get; set; } = default!;
    public string TextFormat { get; set; } = default!;
}