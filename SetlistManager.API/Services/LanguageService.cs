using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Data;
using Microsoft.EntityFrameworkCore;

namespace SetlistManager.API.Services;

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

    public async Task<Data.Entities.Language> GetLanguageByIdAsync(int id) 
        => await _dbContext.Languages
        .FirstAsync(x => x.Id == id);

    public async Task<Data.Entities.Language> GetLanguageByNameAsync(string name)
    => await _dbContext.Languages
        .FirstAsync(x => x.Name.Contains(name));
}
