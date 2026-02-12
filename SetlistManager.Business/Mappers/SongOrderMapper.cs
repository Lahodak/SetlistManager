using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongOrderMapper
{
    public static Setlist MapSongModelToEntity(this SetlistModel setlistModel, Setlist setlist)
    {   
        foreach (var songModel in setlistModel.Songs)
        {            
            setlist.SongsSetlists.Add(new SongsSetlists
            {
                SongId = songModel.Id,
                Order = songModel.Order
            });            
        }

        return setlist;
    }
}