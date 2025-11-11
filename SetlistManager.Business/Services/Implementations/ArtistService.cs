using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly AppDbContext _dbContext;

    public ArtistService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ArtistModel>> GetAllArtistsAsync()
    {
        var artists = await _dbContext.Artists
            .Include(x => x.Songs)!
            .ThenInclude(x => x.Language)            
            .ToListAsync();
        var artistModels = artists.Select(a => a.ToModel(true)).ToList();
        return artistModels;
    }

    public async Task UploadArtistAsync(ArtistModel artistModel)
    {            
        await _dbContext.AddAsync(artistModel.ToEntity());
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ArtistModel> GetArtistModelByIdAsync(int id)
        => (await _dbContext.Artists
        .Include(x => x.Songs)!
        .ThenInclude(x => x.Language)
        .FirstAsync(x => x.Id == id)).ToModel(true);

    public async Task<Artist> GetArtistByIdAsync(int id)
        => await _dbContext.Artists
        .Include(x => x.Songs)!
        .ThenInclude(x => x.Language)
        .FirstAsync(x => x.Id == id);
}