using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IOrderMappingService
{
    Task<SetlistModel> MapSongEntityToModelOrder(Setlist setlist);
    Setlist MapSongModelToEntity(SetlistModel setlistModel, Setlist setlist);
}