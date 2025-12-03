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

        var artistModels = artists
            .Select(a => a.ToModel(true))
            .ToList();
        
        return artistModels;
    }

    public async Task<bool> UploadArtistAsync(ArtistCreateModel createModel)
    {
        if(await _dbContext.Artists.AnyAsync(x => x.Nick == createModel.Nick))            
            return false;

        Artist artist = new()
        {
            Nick = createModel.Nick
        };

        await _dbContext.AddAsync(artist);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<ArtistModel> GetArtistByIdAsync(int id)
        => (await _dbContext.Artists
        .Include(x => x.Songs)!
        .ThenInclude(x => x.Language)
        .FirstAsync(x => x.Id == id))
        .ToModel(true);

    public async Task<bool> TryDeleteArtistAsync(int id)
    {
        var artist = await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == id);
        
        if (artist is null)
            return false;
        
        _dbContext.Artists.Remove(artist);
        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel)
    {
        var artist = await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == id);
        
        if (artist is null)
            return false;
        
        if(await _dbContext.Artists.AnyAsync(x => x.Nick == updateModel.Nick && x.Id != id))
            return false;

        artist.Nick = updateModel.Nick;

        await _dbContext.SaveChangesAsync();
        
        return true;
    }
}