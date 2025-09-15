using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Mappers;

public static class LanguageMapper
{
    public static LanguageModel ToModel(this Language language)
    {
        return new()
        {
            Name = language.Name,
            Code = language.Code,
            Id = language.Id
        };
    }
}