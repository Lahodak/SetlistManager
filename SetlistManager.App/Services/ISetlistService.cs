using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISetlistService
{
    Task SaveSetlistAsync(SetlistModel setlistModel);
    Task<SetlistModel?> GetSetlistById(int id);
    Task<PagedResponse<SetlistModel>?> GetAllSetlistsAsync(PagedRequest request);
    Task EditSetlist(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int id);
    Task<bool> TryGiveAccessToUserAsync(int setlistId, int targetId);
    Task RemoveAccessFromUserAsync(int setlistId, int targetId);
}