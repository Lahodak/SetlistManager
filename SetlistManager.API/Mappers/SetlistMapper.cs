using SetlistManager.Common.Models;

namespace SetlistManager.API.Mappers;

public static class SetlistMapper
{
    public static SetlistModel ToModel(this Data.Entities.Setlist setlist)
    {
        return new SetlistModel
        {
            Id = setlist.Id,
            Name = setlist.Name,
            CreatorId = setlist.CreatorId,
            Songs = setlist.SongsSetlists?
                        .Select(ss => ss.Song.ToModel())
                        .ToList()
                        ?? []
        };
    }
}
