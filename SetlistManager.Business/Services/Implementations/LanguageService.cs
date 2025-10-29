using SetlistManager.Common.Models;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
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

    public async Task<Language> GetLanguageByIdAsync(int id) 
        => await _dbContext.Languages
        .FirstAsync(x => x.Id == id);

    public async Task<Language> GetLanguageByNameAsync(string name)
    => await _dbContext.Languages
        .FirstAsync(x => x.Name.Contains(name));
}
