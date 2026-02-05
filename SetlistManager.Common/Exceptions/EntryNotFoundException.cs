namespace SetlistManager.Common.Exceptions;

public class EntryNotFoundException : Exception
{
    public EntryNotFoundException() : base("Entry not found")
    {
    }

    public EntryNotFoundException(string message) : base(message)
    {
    }
}