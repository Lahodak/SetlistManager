namespace SetlistManager.API.Services;

public interface ICurrentUserContext
{
    int? GetCurrentUserId();
    string? GetCurrentUserEmail();
}
