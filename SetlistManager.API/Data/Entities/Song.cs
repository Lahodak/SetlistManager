namespace SetlistManager.API.Data.Entities;

public class Song : Base
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public string TabsURL { get; set; }
    public string AudioURL { get; set; }
    public string Tuning { get; set; }
    public string Key { get; set; }
    public int BPM { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }

    public int LanguageId { get; set; }
    public Language Language { get; set; }
    public List<SongsSetlists> SongsSetlists { get; set; } 
}
