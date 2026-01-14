using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISetlistsService
{
    Task<SetlistModel?> GetSetlistByIdAsync(int id);
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task<PagedResponse<SetlistModel>?> GetUserSetlistsLibraryAsync(int userId, PagedRequest request);
    Task<bool> TryCreateSetlistAsync(SetlistModel setlistModel, int creatorId);
    Task<bool> TryDeleteSetlistAsync(int id);
    Task<bool> TryGiveAccessToSetlistAsync(int setlistId, int targetId);
}