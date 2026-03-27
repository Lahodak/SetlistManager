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

    public static Song ToEntity(this SongCreateModel model, int creatorId, bool isArtistPublic)
    {
        return new()
        {
            Name = model.Name,
            ArtistId = model.ArtistId!.Value,
            TabsURL = model.TabsURL,
            AudioURL = model.AudioURL,
            Key = model.Key,
            Tuning = model.Tuning,
            BPM = model.BPM!.Value,
            CreatedAt = DateTime.UtcNow,
            OwnerId = creatorId,
            LanguageId = model.LanguageId!.Value,
            IsPublic = isArtistPublic
        };
    }

    public static void UpdateEntity(this Song entity, SongUpdateModel model)
    {
        entity.Name = model.Name;
        entity.ArtistId = model.ArtistId!.Value;
        entity.TabsURL = model.TabsURL;
        entity.AudioURL = model.AudioURL;
        entity.Key = model.Key;
        entity.Tuning = model.Tuning;
        entity.BPM = model.BPM!.Value;
        entity.LanguageId = model.LanguageId!.Value;
        entity.UpdatedAt = DateTime.UtcNow;
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