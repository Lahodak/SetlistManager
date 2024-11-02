namespace SetlistManager.Common.Models;
public class SongModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Artist { get; set; }
    public Language Language { get; set; }
    public string TabsURL { get; set; }
    public string YouTubeURL { get; set; }
}
