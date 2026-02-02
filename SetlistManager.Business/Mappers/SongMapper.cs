using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongMapper
{
    public static SongModel ToModel(this Song entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Artist = entity.Artist.ToModelWithoutSongs(),
            TabsURL = entity.TabsURL,
            AudioURL = entity.AudioURL,
            Language = entity.Language.ToModel(),
            LanguageId = entity.LanguageId,
            Key = entity.Key,
            Tuning = entity.Tuning,
            BPM = entity.BPM,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            OwnerId = entity.OwnerId,
            OwnerNick = entity.Owner?.UserName,
            IsPublic = entity.IsPublic
        };
    }

    public static SongModel ToModelWithoutArtist(this Song entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            TabsURL = entity.TabsURL,
            AudioURL = entity.AudioURL,
            Language = entity.Language.ToModel(),
            LanguageId = entity.LanguageId,
            Key = entity.Key,
            Tuning = entity.Tuning,
            BPM = entity.BPM,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            OwnerId = entity.OwnerId,
            OwnerNick = entity.Owner?.UserName,
            IsPublic = entity.IsPublic
        };
    }
}