using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

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