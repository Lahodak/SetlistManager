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
            IsPublic = artist.IsPublic,
            OwnerId = artist.OwnerId,
            Songs = artist.Songs?
                .Select(s => s.ToModelWithoutArtist())
                .ToList()
        };
    }

    public static ArtistModel ToModelWithoutSongs(this Artist artist)
    {
        return new()
        {
            Id = artist.Id,
            Nick = artist.Nick,
            IsPublic = artist.IsPublic,
            OwnerId = artist.OwnerId,
            Songs = null
        };
    }
}