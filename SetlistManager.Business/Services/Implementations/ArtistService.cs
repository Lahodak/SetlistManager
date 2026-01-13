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

    public async Task<PagedResponse<ArtistModel>> GetPublicArtistsAsync(PagedRequest request)
    {
        var query = _dbContext.Artists
            .Where(x => x.Nick.Contains(request.Query ?? string.Empty) && x.IsPublic);

        var totalCount = await query.CountAsync();

        var artists = await query
           .Include(x => x.Songs)
           .ThenInclude(x => x.Language)
           .AsNoTracking()
           .Skip(request.PageIndex * request.PageSize)
           .Take(request.PageSize)
           .ToListAsync();

        PagedResponse<ArtistModel> response = new()
        {
            TotalCount = totalCount,
            Items = artists
                .Select(a => a.ToModel(true))
                .ToList()
        };
        
        return response;
    }

    public async Task<PagedResponse<ArtistModel>> GetUserArtistLibrary(PagedRequest request, int userId)
    {
        var query = _dbContext.Artists
            .Where(x => x.Nick.Contains(request.Query ?? string.Empty) 
            && (x.OwnerId == userId || x.ArtistsUsers!.Any(x => x.UserId == userId)));

        var totalCount = await query.CountAsync();

        var artists = await query
           .Include(x => x.Songs)!
           .ThenInclude(x => x.Language)
           .Include(x => x.OwnerId)
           .AsNoTracking()
           .Skip(request.PageIndex * request.PageSize)
           .Take(request.PageSize)
           .ToListAsync();

        PagedResponse<ArtistModel> response = new()
        {
            TotalCount = totalCount,
            Items = artists
                .Select(a => a.ToModel(true))
                .ToList()
        };

        return response;
    }

    public async Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel, int creatorId)
    {
        if (await _dbContext.Artists.AnyAsync(x => (!x.IsPublic && x.OwnerId == creatorId && (x.Nick == createModel.Nick)) 
        || (x.ArtistsUsers.Any(x => x.UserId == creatorId) && x.Nick == createModel.Nick)))   
            return false;

        Artist artist = new()
        {
            Nick = createModel.Nick,
            OwnerId = creatorId,
            IsPublic = createModel.IsPublic
        };

        _dbContext.Add(artist);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<ArtistModel?> GetUserArtistById(int artistId, int userId)
    {
        return (await _dbContext.Artists
            .Where(x => x.Id == artistId 
                && (x.OwnerId == userId || x.ArtistsUsers!
                .Any(x => x.UserId == userId)))
            .Include(x => x.Songs
                .Where(x => x.SongsUsers.Any(x => x.UserId == userId) || x.OwnerId == userId))
            .ThenInclude(x => x.Language)
            .FirstOrDefaultAsync())?
            .ToModel(true);
    }

    public async Task<ArtistModel?> GetPublicArtistByIdAsync(int id)
        => (await _dbContext.Artists
        .Include(x => x.Songs.Where(x => x.IsPublic))
        .ThenInclude(x => x.Language)
        .FirstOrDefaultAsync(x => x.Id == id))?
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