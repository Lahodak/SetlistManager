using CsvHelper.Configuration.Attributes;

namespace SetlistManager.Models;
public class Song
{
    [Index(0)]
    public string Name { get; set; }
    [Index(1)]
    public string Artist { get; set; }
    [Index(2)]
    public int SongID { get; set; }
    [Index(3)]
    public Language Language { get; set; }
    [Index(4)]
    public string Tabs { get; set; }
    [Index(5)]
    public string YouTubeURL { get; set; }

	public override string ToString()
	{
		return $"{Name} | {Artist}";
	}
}