namespace SetlistManager.Data.Entities;

public class ArtistsUsers
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public virtual Artist Artist { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}