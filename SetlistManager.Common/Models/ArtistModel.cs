namespace SetlistManager.Common.Models;

public class ArtistModel
{
    public int Id { get; set; }
    public string Nick { get; set; } = default!;
    public int OwnerId { get; set; }
    public bool IsPublic { get; set; }
    public List<SongModel>? Songs { get; set; }
}