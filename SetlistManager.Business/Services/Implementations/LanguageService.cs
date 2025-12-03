using SetlistManager.Common.Models;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data;
using SetlistManager.Business.Mappers;

namespace SetlistManager.Business.Services.Implementations;

public class LanguageService : ILanguageService
{
    private readonly AppDbContext _dbContext;
    public LanguageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LanguageModel>> GetAvailableLanguagesAsync() 
        => (await _dbContext.Languages
        .ToListAsync())
        .Select(x => x.ToModel())
        .ToList();
}
