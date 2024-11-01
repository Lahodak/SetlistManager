namespace SetlistManager.API.Models;
public class Song
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public int SongID { get; set; }
    public Language Language { get; set; }
    public string Tabs { get; set; }
    public string YouTubeURL { get; set; }

    public override string ToString()
    {
        return $"{Name} | {Artist}";
    }
}
