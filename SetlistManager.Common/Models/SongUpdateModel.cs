namespace SetlistManager.Common.Models;

public class SongUpdateModel
{
    public string Name { get; set; } = default!;
    public int ArtistId { get; set; } = default!;
    public string TabsURL { get; set; } = default!;
    public string AudioURL { get; set; } = default!;
    public string Tuning { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int BPM { get; set; } = default!;
    public int LanguageId { get; set; } = default!;
}