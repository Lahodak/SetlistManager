using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extentions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public ArtistService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        if(request.Query is null)
            request.Query = string.Empty;

        var query = _dbContext.Artists
            .Where(x => x.Nick.Contains(request.Query));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == userId || x.ArtistsUsers.Any(su => su.UserId == userId))
            : query.Where(x => x.IsPublic);

        var result = await query
            .Include(x => x.Songs.Where(x => x.OwnerId == userId || x.SongsUsers.Any(su => su.UserId == userId)))
            .ThenInclude(x => x.Language)
            .AsNoTracking()
            .ToPaginatedResultAsync(request);

        return new PagedResponse<ArtistModel>
        {
            TotalCount = result.TotalCount,
            Items = result.Items
                .Select(a => a.ToModel())
                .ToList()
        };
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;
        bool isPublicContent = contentType == ContentType.Public;

        var artist = await _dbContext.Artists
            .Where(x => x.Id == artistId)
            .Where(x => isPublicContent 
                ? x.IsPublic
                : x.OwnerId == userId || x.ArtistsUsers.Any(su => su.UserId == userId))
            .Include(x => x.Songs
                .Where(x => isPublicContent 
                ? x.IsPublic 
                : x.OwnerId == userId || x.SongsUsers.Any(s => s.UserId == userId)))
            .ThenInclude(x => x.Language)
            .FirstOrDefaultAsync();

        if (artist is null)
            throw new EntryNotFoundException($"Artist with Id '{artistId}' not found or you don't have access");

        return artist.ToModel();
    }

    public async Task TryCreateArtistAsync(ArtistCreateModel createModel)
    {
        int creatorId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Artists.AnyAsync(x => (!x.IsPublic && x.OwnerId == creatorId && (x.Nick == createModel.Nick)) 
        || (x.ArtistsUsers.Any(x => x.UserId == creatorId) && x.Nick == createModel.Nick)))   
            throw new DuplicateEntryException();

        Artist artist = new()
        {
            Nick = createModel.Nick,
            OwnerId = creatorId,
            IsPublic = createModel.IsPublic
        };

        _dbContext.Add(artist);
        await _dbContext.SaveChangesAsync();        
    }

    public async Task TryDeleteArtistAsync(int artistId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var artist = await _dbContext.Artists
            .Include(x => x.Owner)
            .Include(x => x.Songs)
            .FirstOrDefaultAsync(x => x.Id == artistId);
        
        if (artist is null || artist.OwnerId != userId || artist.Songs.Any(x => x.IsPublic) || artist.IsPublic)
            throw new EntryNotFoundException();       

        _dbContext.Artists.Remove(artist);        
        await _dbContext.SaveChangesAsync();
    }

    public async Task TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var artist = await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == currentUserId);
        
        if (artist is null || await _dbContext.Artists.AnyAsync(x => x.Nick == updateModel.Nick && x.Id != id))
            throw new EntryNotFoundException();

        artist.Nick = updateModel.Nick;
        await _dbContext.SaveChangesAsync();        
    }

    public async Task TryMakeArtistPublicAsync(int artistId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var artist = await _dbContext.Artists
            .FirstOrDefaultAsync(x => x.Id == artistId && x.OwnerId == currentUserId);

        if (artist is null)
            throw new EntryNotFoundException();

        artist.IsPublic = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task TryGiveAccessToUserAsync(int artistId, int targetId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var artist = await _dbContext.Artists
            .Include(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .FirstOrDefaultAsync(x => x.Id == artistId);

        if (artist is null || artist.ArtistsUsers.Count != 0 || (currentUserId != artist.OwnerId && targetId != currentUserId))
            throw new EntryNotFoundException();

        ArtistsUsers artistsUsers = new()
        {
            ArtistId = artistId,
            UserId = targetId
        };

        _dbContext.ArtistsUsers.Add(artistsUsers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int artistId, int targetId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var artistUser = await _dbContext.ArtistsUsers
            .Include(x => x.Artist)
            .Include(x => x.User)
                .ThenInclude(x => x.Songs.Where(x => x.ArtistId == artistId))
            .FirstOrDefaultAsync(x => x.ArtistId == artistId && x.UserId == targetId);
        
        if (artistUser is null || (currentUserId != artistUser.Artist.OwnerId && targetId != currentUserId) || artistUser.User.Songs.Count != 0)
            throw new EntryNotFoundException();

        _dbContext.ArtistsUsers.Remove(artistUser);
        await _dbContext.SaveChangesAsync();
    }
}