using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongOrderMapper
{
    public static SetlistModel MapSongEntityToModelWithOrder(this Setlist setlist)
    {
        var setlistModel = setlist.ToModel();

        foreach (var songSetlist in setlist.SongsSetlists)
        {
            setlistModel.Songs.First(s => s.Id == songSetlist.SongId).Order = songSetlist.Order;
        }
        
        setlistModel.Songs = setlistModel.Songs
            .OrderBy(x => x.Order)
            .ToList();

        return setlistModel;
    }

    public static Setlist MapSongModelToEntity(this SetlistModel setlistModel, Setlist setlist)
    {   
        foreach (var songModel in setlistModel.Songs)
        {            
            setlist.SongsSetlists.Add(new SongsSetlists
            {
                SongId = songModel.Id,
                SetlistId = setlist.Id,
                Order = songModel.Order
            });            
        }

        return setlist;
    }
}