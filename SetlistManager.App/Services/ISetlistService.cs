using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISetlistService
{
    Task PushSetlist(SetlistModel setlistModel);
    Task<SetlistModel?> GetSetlistById(int id);
    Task<List<SetlistModel>?> GetAllSetlistsAsync();
    Task EditSetlist(SetlistModel setlistModel);
    Task<bool> TryDeleteSetlistAsync(int id);
}