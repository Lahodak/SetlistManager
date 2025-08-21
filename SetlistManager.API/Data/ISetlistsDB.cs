using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public interface ISetlistsDB
{
    Task<SetlistModel?> GetSetlistByIdAsync (int id);
    Task<SetlistModel?> GetSetlistByNameAsync(string name);
    Task<IEnumerable<SetlistModel>> GetAllSetlistsOfUserAsync(int userId);
    Task<int> SaveSetlistAsync (SetlistModel setlistModel);
    Task<IEnumerable<SetlistModel>> GetAllSetlistsAsync();
    Task<bool> EditSetlistAsync(SetlistModel setlistModel);

}