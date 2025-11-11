namespace SetlistManager.Common.Models;
public class SongModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ArtistModel Artist { get; set; } = new();
    public string TabsURL { get; set; } = string.Empty;
    public string AudioURL { get; set; } = string.Empty;
    public string Tuning { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int BPM { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public LanguageModel Language { get; set; } = new();
    public int LanguageId { get; set; }
    public int Order { get; set; }
}