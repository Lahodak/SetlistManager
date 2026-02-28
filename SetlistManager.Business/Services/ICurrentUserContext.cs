namespace SetlistManager.Business.Services;

public interface ICurrentUserContext
{
    int? GetCurrentUserId();
    string? GetCurrentUserEmail();
    public int UserId { get; set; }
}
