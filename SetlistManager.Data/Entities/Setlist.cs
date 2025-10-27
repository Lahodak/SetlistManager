using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public class Setlist : Base
{
    [Required]
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }    
    public int CreatorId { get; set; }
    public virtual required User Creator { get; set; }
    public virtual required List<SongsSetlists> SongsSetlists { get; set; } = [];
    public virtual List<Room>? Rooms { get; set; }
}