using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISetlistsService
{
    Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request);
    Task<SetlistModel> GetSetlistByIdAsync(int setlistId);
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task CreateSetlistAsync(SetlistCreateModel createModel);
    Task DeleteSetlistAsync(int setlistId);
    Task GiveAccessToSetlistAsync(int setlistId, int targetId);
    Task RemoveAccessFromUserAsync(int setlistId, int userId);
}