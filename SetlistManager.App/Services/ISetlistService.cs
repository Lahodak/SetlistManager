using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISetlistService
{
    Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request);
    Task<SetlistModel?> GetSetlistById(int id);
    Task<bool> TryCreateSetlistAsync(SetlistCreateModel createModel);
    Task<bool> TryEditSetlist(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int id);
    Task<bool> TryGiveAccessToUserAsync(int setlistId, int targetId);
    Task<bool> TryRemoveAccessFromUserAsync(int setlistId, int targetId);
}