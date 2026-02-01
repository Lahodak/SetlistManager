namespace SetlistManager.Common.Exceptions;

public class GeniusSongLyricsNotFoundException : Exception
{
    public GeniusSongLyricsNotFoundException() : base("SetlistManager didn't recieve Lyrics from Genius.")
    {
    }
}