namespace SetlistManager.Data.Entities;

public class Setlist : BaseEntity
{
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = default!;
    public virtual List<Room> Rooms { get; set; } = [];
    public virtual List<SetlistsUsers> SetlistsUsers { get; set; } = [];
    public virtual List<SongsSetlists> SongsSetlists { get; set; } = [];
}