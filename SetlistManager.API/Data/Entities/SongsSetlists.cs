namespace SetlistManager.API.Data.Entities;

public class SongsSetlists : Base
{
    public int SongId { get; set; }
    public virtual Song Song { get; set; }

    public int SetlistId { get; set; }
    public virtual Setlist Setlist { get; set; }

    public int Order { get; set; }
}