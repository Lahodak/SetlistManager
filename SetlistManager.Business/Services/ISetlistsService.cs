using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISetlistsService
{
    Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request);
    Task<SetlistModel> GetSetlistByIdAsync(int setlistId);
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task TryCreateSetlistAsync(SetlistModel setlistModel);
    Task TryDeleteSetlistAsync(int setlistId);
    Task TryGiveAccessToSetlistAsync(int setlistId, int targetId);
    Task RemoveAccessFromUserAsync(int setlistId, int userId);
}