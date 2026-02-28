namespace SetlistManager.Common.Exceptions;

public class GeniusSongLyricsNotFoundException : Exception
{
    public GeniusSongLyricsNotFoundException() : base("SetlistManager didn't receive Lyrics from Genius.")
    {
    }
}