namespace SetlistManager.Data.Entities;

public class SongsUsers
{
    public int Id { get; set; }
    public int SongId { get; set; }
    public virtual Song Song { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}