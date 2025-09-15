using SetlistManager.Common.Models;

namespace SetlistManager.API.Services;

public interface IInstrumentsService
{
    Task<List<InstrumentModel>> GetAvailableInstrumentsAsync();
}