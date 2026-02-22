using SetlistManager.Business.Mappers;
using SetlistManager.Data.Entities;

namespace SetlistManager.Tests;

public class InstrumentMapperTests
{
    private static Instrument CreateInstrument() => new()
    {
        Id = 1,
        Name = "Guitar"
    };

    [Fact]
    public void ToModel_MapsAllProperties()
    {
        var instrument = CreateInstrument();

        var result = instrument.ToModel();

        Assert.Equal(instrument.Id, result.Id);
        Assert.Equal(instrument.Name, result.Name);
    }

    [Fact]
    public void ToModel_WithDifferentInstrument_MapsCorrectly()
    {
        var instrument = new Instrument { Id = 3, Name = "Drums" };

        var result = instrument.ToModel();

        Assert.Equal(3, result.Id);
        Assert.Equal("Drums", result.Name);
    }
}