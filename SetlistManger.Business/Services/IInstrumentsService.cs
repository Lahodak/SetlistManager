using SetlistManager.Common.Models;

namespace SetlistManger.Business.Services;

public interface IInstrumentsService
{
    Task<List<InstrumentModel>> GetAvailableInstrumentsAsync();
}