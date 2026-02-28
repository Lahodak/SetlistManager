using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongOrderMapper
{
    public static Setlist MapCreateModelToEntity(this SetlistCreateModel createModel, Setlist setlist)
    {
        foreach (var song in createModel.Songs)
        {
            setlist.SongsSetlists.Add(new SongsSetlists
            {
                SongId = song.SongId,
                Order = song.Order
            });
        }

        return setlist;
    }
}