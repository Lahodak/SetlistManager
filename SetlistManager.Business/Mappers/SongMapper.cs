using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongMapper
{
    public static SongModel ToModel(this Song entity, bool includeArtist = true)
    {
        SongModel model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Artist = includeArtist ? entity.Artist.ToModel(false) : new(),
            TabsURL = entity.TabsURL,
            AudioURL = entity.AudioURL,
            Language = entity.Language.ToModel(),
            LanguageId = entity.LanguageId,
            Key = entity.Key,
            Tuning = entity.Tuning,
            BPM = entity.BPM,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
        return model;
    }
}