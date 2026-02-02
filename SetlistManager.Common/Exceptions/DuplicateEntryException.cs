namespace SetlistManager.Common.Exceptions;

public class DuplicateEntryException : Exception
{
    public DuplicateEntryException() : base("Entry with provided values already exists.")
    {
    }
}