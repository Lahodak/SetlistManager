namespace SetlistManager.Data.Entities;

public class SongsUsers : Base
{
    public int SongId { get; set; }
    public virtual Song Song { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}