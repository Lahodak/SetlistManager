using SetlistManager.Common.Models;
namespace SetlistManger.Business.Services;
public interface ISetlistsService
{
    Task<SetlistModel?> GetSetlistByIdAsync (int id);
    Task<SetlistModel?> GetSetlistByNameAsync(string name);
    Task<IEnumerable<SetlistModel>?> GetAllSetlistsOfUserAsync(int userId);
    Task SaveSetlistAsync(SetlistModel setlistModel);
    Task<IEnumerable<SetlistModel>?> GetAllSetlistsAsync();
    Task EditSetlistAsync(SetlistModel setlistModel);
}