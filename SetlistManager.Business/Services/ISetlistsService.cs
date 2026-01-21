using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISetlistsService
{
    Task<SetlistModel?> GetSetlistByIdAsync(int setlistId, int userId);
    Task EditSetlistAsync(SetlistModel setlistModel, int currentUserId);
    Task<PagedResponse<SetlistModel>?> GetSetlistsAsync(int userId, PagedRequest request);
    Task<bool> TryCreateSetlistAsync(SetlistModel setlistModel, int creatorId);
    Task<bool> TryDeleteSetlistAsync(int setlistId, int currentUserId);
    Task<bool> TryGiveAccessToSetlistAsync(int setlistId, int targetId, int currentUserId);
    Task RemoveAccessFromUserAsync(int setlistId, int userId, int currentUserId);
}