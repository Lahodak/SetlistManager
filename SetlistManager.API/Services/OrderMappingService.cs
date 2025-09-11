using SetlistManager.API.Data;
using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Services;

public class OrderMappingService
{
    private readonly AppDbContext _dbContext;
    public OrderMappingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public SetlistModel MapSongEntityToModelOrder(Setlist setlist)
    {
        var setlistModel = setlist.ToModel();

        foreach (var songSetlist in setlist.SongsSetlists)
        {
            setlistModel.Songs.First(s => s.Id == songSetlist.SongId).Order = songSetlist.Order;
        }
        setlistModel.Songs = setlistModel.Songs.OrderBy(x => x.Order).ToList();

        return setlistModel;
    }

    public Setlist MapSongModelToEntity(SetlistModel setlistModel, Setlist setlist)
    {   
        foreach (var songModel in setlistModel.Songs)
        {
            var songEntity = _dbContext.Songs.FirstOrDefault(s => s.Id == songModel.Id);
            if (songEntity != null)
            {
                setlist.SongsSetlists.Add(new SongsSetlists
                {
                    SongId = songEntity.Id,
                    SetlistId = setlist.Id,
                    Order = songModel.Order
                });
            }
        }

        return setlist;
    }
}