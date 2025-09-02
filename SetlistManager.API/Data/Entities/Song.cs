using SetlistManager.Common.Models;

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
   
    public SongModel ToModel()
    {
        SongModel model = new()
        {
            Id = Id,
            Name = Name,
            Artist = Artist,
            TabsURL = TabsURL,
            AudioURL = AudioURL,
            LanguageId = LanguageId,
            Key = Key,
            Tuning = Tuning,
            BPM = BPM,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            UpdatedBy = UpdatedBy,
        };

        return model;
    }
}
