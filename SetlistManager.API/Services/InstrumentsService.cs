using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data;
using SetlistManager.Common.Models;
using SetlistManager.API.Mappers;

namespace SetlistManager.API.Services;

public class InstrumentsService : IInstrumentsService
{
    private readonly AppDbContext _appDbContext;
    public InstrumentsService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<InstrumentModel>> GetAvailableInstrumentsAsync()
    {
        var instuments = await _appDbContext.Instruments.ToListAsync();

        return instuments
            .Select(x => x.ToModel())
            .ToList();
    }
}
