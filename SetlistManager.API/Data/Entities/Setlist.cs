using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class Setlist : Base
{
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int CreatorId { get; set; }
    public required User Creator { get; set; }
    public int UpdatedBy { get; set; }
    public List<Room> Rooms { get; set; }
    public List<SongsSetlists> SongsSetlists { get; set; }
    public List<RoomsSetlists> RoomsSetlists { get; set; }

    public SetlistModel ToModel()
    {
        return new SetlistModel
        {
            Id = Id,
            Name = Name,
            CreatorId = CreatorId,
            Songs = SongsSetlists?
                        .Select(ss => ss.Song.ToModel())
                        .ToList()
                        ?? new List<SongModel>()
        };
    }
}