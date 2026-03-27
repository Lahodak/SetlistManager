using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides methods for managing instruments, including retrieving available instruments for setlists.
/// </summary>
public interface IInstrumentsService
{
    /// <summary>
    /// Retrieves a list of available instruments that can be used in setlists.
    /// </summary>
    Task<List<InstrumentModel>> GetAvailableInstrumentsAsync();
}