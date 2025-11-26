using SetlistManager.Common.Models;
namespace SetlistManager.Business.Services;
public interface ISetlistsService
{
    Task<SetlistModel?> GetSetlistByIdAsync(int id);
    Task<IEnumerable<SetlistModel>?> GetAllSetlistsOfUserAsync(int userId);
    Task SaveSetlistAsync(SetlistModel setlistModel);
    Task<IEnumerable<SetlistModel>?> GetAllSetlistsAsync();
    Task EditSetlistAsync(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int id);
}