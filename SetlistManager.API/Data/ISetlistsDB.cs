using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public interface ISetlistsDB
{
    Task<SetlistModel?> GetSetlistById (int id);
    Task<int> SaveSetlist (SetlistModel setlistModel);
}