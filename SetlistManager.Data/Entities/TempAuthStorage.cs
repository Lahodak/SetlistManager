namespace SetlistManager.Data.Entities;

public class TempAuthStorage : Base
{
    public string TempSalt { get; set; } = default!;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}