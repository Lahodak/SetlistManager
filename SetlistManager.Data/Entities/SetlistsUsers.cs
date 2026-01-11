namespace SetlistManager.Data.Entities;

public class SetlistsUsers
{
    public int Id { get; set; }
    public int SetlistId { get; set; }
    public virtual Setlist Setlist { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}