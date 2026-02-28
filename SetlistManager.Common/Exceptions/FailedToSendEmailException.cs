namespace SetlistManager.Common.Exceptions;

public class FailedToSendEmailException : Exception
{
    public FailedToSendEmailException(string message) : base(message)
    {
    }
}
