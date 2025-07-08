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
    public List<Song> Songs { get; set; }
    public List<SongsSetlists> SongsSetlists { get; set; }
    public List<RoomsSetlists> RoomsSetlists { get; set; }
}