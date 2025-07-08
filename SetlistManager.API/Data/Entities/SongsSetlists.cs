namespace SetlistManager.API.Data.Entities;

public class SongsSetlists
{
    public int SongId { get; set; }
    public Song Song { get; set; } = default!;

    public int SetlistId { get; set; }
    public Setlist Setlist { get; set; } = default!;
}
