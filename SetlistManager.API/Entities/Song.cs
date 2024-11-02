using SetlistManager.Common.Models;

namespace SetlistManager.API.Entities;
public class Song
{
    public required string Name { get; set; }
    public required string Artist { get; set; }
    public int Id { get; set; }
    public Language Language { get; set; }
    public required string TabsURL { get; set; }
    public required string YouTubeURL { get; set; }

    public override string ToString()
    {
        return $"{Name} | {Artist}";
    }
}
