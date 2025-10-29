namespace SetlistManager.Data.Entities;

public class TempAuthStorage : Base
{
    public string TempSecret { get; set; } = default!;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}