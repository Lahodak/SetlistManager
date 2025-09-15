using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Mappers;

public static class InstrumentMapper
{
    public static InstrumentModel ToModel(this Instrument instrument)
    {
        return new()
        {
            Name = instrument.Name,
            Id = instrument.Id
        };
    }
}
