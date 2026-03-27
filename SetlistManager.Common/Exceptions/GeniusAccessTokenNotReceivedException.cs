namespace SetlistManager.Common.Exceptions;

public class GeniusAccessTokenNotReceivedException : Exception
{
    public GeniusAccessTokenNotReceivedException() : base("SetlistManager didn't receive Access Token from Genius.")
    {
    }
}