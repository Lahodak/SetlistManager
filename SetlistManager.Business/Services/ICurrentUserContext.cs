namespace SetlistManager.Business.Services;

/// <summary>
/// Provides access to information about the currently authenticated user, such as their ID and email address.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// Gets the ID of the currently authenticated user, or null if no user is authenticated.
    /// </summary>
    int? GetCurrentUserId();

    /// <summary>
    /// Gets the email address of the currently authenticated user, or null if no user is authenticated.
    /// </summary>
    string? GetCurrentUserEmail();
    public int UserId { get; set; }
}