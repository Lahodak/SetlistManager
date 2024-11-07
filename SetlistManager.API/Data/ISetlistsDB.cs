using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public interface ISetlistsDB
{
    Task<SetlistModel?> GetSetlistById (string setId);
    Task SaveSetlist(SetlistModel setlistModel);
}