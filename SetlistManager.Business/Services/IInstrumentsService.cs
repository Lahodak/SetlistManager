using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IInstrumentsService
{
    Task<List<InstrumentModel>> GetAvailableInstrumentsAsync();
}