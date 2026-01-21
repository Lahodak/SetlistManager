namespace SetlistManager.Common.Models;

public class SongUsageStatModel
{
    public int SongId { get; set; }
    public string Name { get; set; } = default!;
    public int UsageCount { get; set; }
}