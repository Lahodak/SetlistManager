namespace SetlistManager.Common.Models;

public class SongCreateModel
{
    public string Name { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public int LanguageId { get; set; }
    public string TabsURL { get; set; } = string.Empty;
    public string AudioURL { get; set; } = string.Empty;
    public string Tuning { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int BPM { get; set; }
}