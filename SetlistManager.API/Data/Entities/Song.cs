using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class Song : Base
{
    public string Name { get; set; } = default!;
    public string Artist { get; set; } = default!;
    public string TabsURL { get; set; } = default!;
    public string AudioURL { get; set; } = default!;
    public string Tuning { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int BPM { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public int LanguageId { get; set; }
    public virtual required Language Language { get; set; }
    public virtual List<SongsSetlists>? SongsSetlists { get; set; } 
   
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
            UpdatedBy = UpdatedBy
        };

        return model;
    }
}