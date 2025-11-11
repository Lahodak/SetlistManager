using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public class Setlist : Base
{
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }    
    public int CreatorId { get; set; }
    public virtual User Creator { get; set; } = default!;
    public virtual List<SongsSetlists> SongsSetlists { get; set; } = [];
    public virtual List<Room>? Rooms { get; set; }
}