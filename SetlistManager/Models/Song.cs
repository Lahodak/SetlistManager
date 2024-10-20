using CsvHelper.Configuration.Attributes;

namespace SetlistManager.Models;
public class Song
{
    [Index(0)]
    public required string Name { get; set; }
    [Index(1)]
    public required string Artist { get; set; }
    [Index(2)]
    public required int SongID { get; set; }
    [Index(3)]
    public required Language Language { get; set; }
    [Index(4)]
    public required string Tabs { get; set; }

	public override string ToString()
	{
		return $"{Name} | {Artist}";
	}

	public static List<Song> GetExampleSongCollection()
    {
        List<Song> list = [];
        Song song1 = new()
        {
            Name = "Siuu",
            Artist = "Ronaldo Kristioano",
            Language = Language.EN,
            SongID = 1,
            Tabs = "https://www.google.com"
        };
        Song song2 = new()
        { 
            Name = "Skibiďácký toilet",
            Artist = "Netuším jak se blud jmenuje",
            Language = Language.CZ,
            SongID = 2,
            Tabs = "https://www.google.com"
        };
        Song song3 = new()
        {
            Name = "Slovenská hitovka",
            Artist = "Unknown Artist",
            Language = Language.SK,
            SongID = 3,
            Tabs = "https://www.google.com"
        };
        Song song4 = new()
        {
            Name = "Bohemian Rhapsody",
            Artist = "Queen",
            Language = Language.EN,
            SongID = 4,
            Tabs = "https://www.google.com"
        };
        Song song5 = new()
        {
            Name = "Holubí dům",
            Artist = "Jiří Schelinger",
            Language = Language.CZ,
            SongID = 5,
            Tabs = "https://www.google.com"
        };
        Song song6 = new()
        {
            Name = "Tears in Heaven",
            Artist = "Eric Clapton",
            Language = Language.EN,
            SongID = 6,
            Tabs = "https://www.google.com"
        };
        Song song7 = new()
        {
            Name = "Prší, prší",
            Artist = "Lidová písnička",
            Language = Language.SK,
            SongID = 7,
            Tabs = "https://www.google.com"
        };
        Song song8 = new()
        {
            Name = "Rolling In The Deep",
            Artist = "Adele",
            Language = Language.EN,
            SongID = 8,
            Tabs = "https://www.google.com"
        };
        list.Add(song1);
        list.Add(song2);
        list.Add(song3);
        list.Add(song4);
        list.Add(song5);
        list.Add(song6);
        list.Add(song7);
        list.Add(song8);
        return list;
    }
}