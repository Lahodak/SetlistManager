namespace SetlistManager.Common.Exceptions;

public class GeniusAccessTokenNotRecievedException : Exception
{
    public GeniusAccessTokenNotRecievedException() : base("SetlistManager didn't recieve Access Token from Genius.")
    {
    }
}