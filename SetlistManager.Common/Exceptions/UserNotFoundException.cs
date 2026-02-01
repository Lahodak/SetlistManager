namespace SetlistManager.Common.Exceptions;

public class UserNotFoundException : Exception
{
    public int UserId { get; }

    public UserNotFoundException() : base($"User not found.")
    {
    }

    public UserNotFoundException(int userId)
        : base($"User with ID {userId} was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(int userId, string message)
        : base(message)
    {
        UserId = userId;
    }

    public UserNotFoundException(int userId, string message, Exception innerException)
        : base(message, innerException)
    {
        UserId = userId;
    }
}