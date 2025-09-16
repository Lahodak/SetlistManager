using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class SongMapper
{
    public static SongModel ToModel(this Song entity)
    {
        SongModel model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Artist = entity.Artist,
            TabsURL = entity.TabsURL,
            AudioURL = entity.AudioURL,
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