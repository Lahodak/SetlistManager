using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SetlistMapper
{
    public static SetlistModel ToModel(this Setlist setlist)
    {
        return new SetlistModel
        {
            Id = setlist.Id,
            Name = setlist.Name,            
            OwnerId = setlist.OwnerId,
            Songs = setlist.SongsSetlists?
                        .Select(ss => 
                        {
                            var model = ss.Song.ToModel();
                            model.Order = ss.Order;                            
                            return model;
                        })
                        .OrderBy(s => s.Order)
                        .ToList()
                        ?? []
        };
    }
}