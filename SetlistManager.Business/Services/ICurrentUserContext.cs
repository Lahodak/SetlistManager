namespace SetlistManager.Business.Services;

public interface ICurrentUserContext
{
    int? GetCurrentUserId();
    string? GetCurrentUserEmail();
}
