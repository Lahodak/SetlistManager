using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISetlistService
{
    Task PushSetlist(SetlistModel setlistModel);
    Task<SetlistModel?> GetSetlistById(int id);
    Task<List<SetlistModel>?> GetAllSetlistsAsync();
    Task<SetlistModel?> GetSetlistByNameAsync(string name);
    Task EditSetlist(SetlistModel setlistModel);
}