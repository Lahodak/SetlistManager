using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public interface ISetlistsDB
{
    Task<SetlistModel?> GetSetlistByIdAsync (int id);
    Task<SetlistModel?> GetSetlistByNameAsync(string name);

    Task<int> SaveSetlistAsync (SetlistModel setlistModel);
}