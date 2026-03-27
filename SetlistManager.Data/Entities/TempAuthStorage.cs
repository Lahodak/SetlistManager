namespace SetlistManager.Data.Entities;

public class TempAuthStorage : BaseEntity
{
    public string TempSecret { get; set; } = default!;
    public User User { get; set; } = default!;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}