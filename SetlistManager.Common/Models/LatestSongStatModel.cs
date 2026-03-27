namespace SetlistManager.Common.Models;

public class LatestSongStatModel
{
    public int SongId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string ArtistNick { get; set; } = default!;
}