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

    public async Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request, int userId)
    {
        var searchQuery = request.Query ?? string.Empty;
        var query = _dbContext.Artists
            .Where(x => x.Nick.Contains(searchQuery));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == userId || x.ArtistsUsers.Any(su => su.UserId == userId))
            : query.Where(x => x.IsPublic);

        var totalCount = await query.CountAsync();

        var artists = await query
           .Include(x => x.Songs)
           .ThenInclude(x => x.Language)
           .AsNoTracking()
           .Skip(request.PageIndex * request.PageSize)
           .Take(request.PageSize)
           .AsNoTracking()
           .ToListAsync();

        return new PagedResponse<ArtistModel>
        {
            TotalCount = totalCount,
            Items = artists
                .Select(a => a.ToModel(true))
                .ToList()
        };
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int artistId, int userId, ContentType contentType)
    {
        var artist = await _dbContext.Artists
            .Where(x => x.Id == artistId)
            .Where(x => contentType == ContentType.Public
                ? x.IsPublic
                : x.OwnerId == userId || x.ArtistsUsers.Any(su => su.UserId == userId))
            .Include(x => x.Songs
                .Where(x => contentType == ContentType.Public
                    ? x.IsPublic
                    : x.SongsUsers.Any(s => s.UserId == userId) || x.OwnerId == userId))
            .ThenInclude(x => x.Language)
            .FirstOrDefaultAsync();

        return artist?.ToModel(true);
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

    public async Task<bool> TryDeleteArtistAsync(int artistId, int userId)
    {
        var artist = await _dbContext.Artists
            .Include(x => x.Owner)
            .Include(x => x.Songs)
            .FirstOrDefaultAsync(x => x.Id == artistId);
        
        if (artist is null || artist.OwnerId != userId || artist.Songs.Any(x => x.IsPublic) || artist.IsPublic)
            return false;       

        _dbContext.Artists.Remove(artist);        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel, int currentUserId)
    {
        var artist = await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == currentUserId);
        
        if (artist is null || await _dbContext.Artists.AnyAsync(x => x.Nick == updateModel.Nick && x.Id != id))
            return false;

        artist.Nick = updateModel.Nick;

        await _dbContext.SaveChangesAsync();        
        return true;
    }

    public async Task<bool> TryMakeArtistPublicAsync(int artistId, int currentUserId)
    {
        var artist = await _dbContext.Artists
            .FirstOrDefaultAsync(x => x.Id == artistId && x.OwnerId == currentUserId);

        if (artist is null)
            return false;

        artist.IsPublic = true;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId, int currentUserId)
    {
        var artist = await _dbContext.Artists
            .Include(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .FirstOrDefaultAsync(x => x.Id == artistId);

        if (artist is null || artist.ArtistsUsers.Count != 0 || (currentUserId != artist.OwnerId && targetId != currentUserId))
            return false;

        ArtistsUsers artistsUsers = new()
        {
            ArtistId = artistId,
            UserId = targetId
        };

        _dbContext.ArtistsUsers.Add(artistsUsers);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task RemoveAccessFromUserAsync(int artistId, int targetId, int currentUserId)
    {
        var artistUser = await _dbContext.ArtistsUsers
            .Include(x => x.Artist)
            .Include(x => x.User)
                .ThenInclude(x => x.Songs.Where(x => x.ArtistId == artistId))
            .FirstOrDefaultAsync(x => x.Id == artistId && x.UserId == targetId);
        
        if (artistUser is null || (currentUserId != artistUser.Artist.OwnerId && targetId != currentUserId) || artistUser.User.Songs.Count != 0)
            return;

        _dbContext.ArtistsUsers.Remove(artistUser);
        await _dbContext.SaveChangesAsync();
    }
}