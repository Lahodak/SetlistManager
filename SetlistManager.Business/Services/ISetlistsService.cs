using SetlistManager.Common.Models;
namespace SetlistManager.Business.Services;
public interface ISetlistsService
{
    Task<SetlistModel?> GetSetlistByIdAsync(int id);
    Task<IEnumerable<SetlistModel>?> GetAllSetlistsOfUserAsync(int userId);
    Task SaveSetlistAsync(SetlistModel setlistModel);
    Task<PagedResponse<SetlistModel>> GetAllSetlistsAsync(PagedRequest request);
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int id);
}