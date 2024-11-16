namespace SetlistManager.Common.Models;
public class SongModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public Language Language { get; set; }
    public string TabsURL { get; set; } = string.Empty;
    public string YouTubeURL { get; set; } = string.Empty;   
}