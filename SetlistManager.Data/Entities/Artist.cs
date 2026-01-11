namespace SetlistManager.Data.Entities;

public  class Artist : Base
{
    public string Nick { get; set; } = default!;
    public bool IsPublic { get; set; }
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = default!;
    public virtual List<Song>? Songs { get; set; } = [];
    public virtual List<ArtistsUsers>? ArtistsUsers { get; set; } = [];
}