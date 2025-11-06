using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IInstrumentService
{
    Task<List<InstrumentModel>?> GetAvailableInstrumentsAsync();
}