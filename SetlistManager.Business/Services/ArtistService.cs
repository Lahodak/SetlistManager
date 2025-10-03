using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;

namespace SetlistManager.Business.Services;

public class ArtistService : IArtistService
{
    private readonly AppDbContext _dbContext;

    public ArtistService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ArtistModel>> GetAllArtistsAsync() 
        => (await _dbContext.Artists
        .Include(x => x.Songs)
        .ToListAsync())
        .Select(a => a.ToModel())
        .ToList();

    public async Task UploadArtistAsync(ArtistModel artistModel)
    {            
        await _dbContext.AddAsync(artistModel.ToEntity());
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ArtistModel> GetArtistByIdAsync(int id)
        => (await _dbContext.Artists
        .Include(x => x.Songs)
        .FirstAsync(x => x.Id == id))
        .ToModel();
}