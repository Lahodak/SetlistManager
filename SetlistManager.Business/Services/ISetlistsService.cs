using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISetlistsService
{
    Task<PagedResponse<SetlistModel>?> GetSetlistsAsync(PagedRequest request);
    Task<SetlistModel?> GetSetlistByIdAsync(int setlistId);
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task<bool> TryCreateSetlistAsync(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int setlistId);
    Task<bool> TryGiveAccessToSetlistAsync(int setlistId, int targetId);
    Task RemoveAccessFromUserAsync(int setlistId, int userId);
}