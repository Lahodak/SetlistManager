namespace SetlistManager.API.Data.Entities;

public class Room : Base
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }    
    public int HostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public int CurrentSongId { get; set; }
    public int? SetlistId { get; set; }
    public virtual Setlist? Setlist { get; set; }
    public virtual List<User> Users { get; set; } = [];
}