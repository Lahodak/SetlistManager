namespace SetlistManager.Data.Entities;

public class SongsSetlists : Base
{
    public int SongId { get; set; }
    public virtual Song Song { get; set; } = default!;

    public int SetlistId { get; set; }
    public virtual Setlist Setlist { get; set; } = default!;

    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}