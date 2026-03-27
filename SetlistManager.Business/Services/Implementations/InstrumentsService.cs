using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;
using SetlistManager.Business.Mappers;
using SetlistManager.Data;

namespace SetlistManager.Business.Services.Implementations;

public class InstrumentsService : IInstrumentsService
{
    private readonly AppDbContext _dbContext;
    public InstrumentsService(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public async Task<List<InstrumentModel>> GetAvailableInstrumentsAsync()
    {
        var instuments = await _dbContext.Instruments.ToListAsync();

        return instuments
            .Select(x => x.ToModel())
            .ToList();
    }
}