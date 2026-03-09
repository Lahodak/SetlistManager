using SetlistManager.Business.Mappers;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Mappers;

public class LanguageMapperTests
{
    private static Language CreateLanguage() => new()
    {
        Id = 1,
        Name = "English",
        Code = "EN"
    };

    [Fact]
    public void ToModel_MapsAllProperties()
    {
        var language = CreateLanguage();

        var result = language.ToModel();

        Assert.Equal(language.Id, result.Id);
        Assert.Equal(language.Name, result.Name);
        Assert.Equal(language.Code, result.Code);
    }

    [Fact]
    public void ToModel_WithDifferentLanguage_MapsCorrectly()
    {
        var language = new Language { Id = 5, Name = "Czech", Code = "CS" };

        var result = language.ToModel();

        Assert.Equal(5, result.Id);
        Assert.Equal("Czech", result.Name);
        Assert.Equal("CS", result.Code);
    }
}