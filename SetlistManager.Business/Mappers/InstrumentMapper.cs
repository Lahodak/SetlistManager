using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

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
