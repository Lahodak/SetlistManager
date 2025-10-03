using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class ArtistMapper
{
    public static ArtistModel ToModel(this Artist artist)
    {
        return new()
        {
            Id = artist.Id,
            Nick = artist.Nick,
            Songs = artist.Songs?
                        .Select(ss => ss.ToModel())
                        .ToList()
        };
    }

    public static Artist ToEntity(this ArtistModel model)
    {
        return new()
        {
            Nick = model.Nick
        };
    }
}