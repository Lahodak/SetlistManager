using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extensions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly AppDbContext _dbContext;
    private readonly int _currentUserId;

    public ArtistService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserId = currentUserContext.UserId;
    }

    public async Task<PagedResponse<ArtistModel>> GetArtistsAsync(ContentPagedRequest request)
    {
        if (request.Query is null)
            request.Query = string.Empty;

        var query = _dbContext.Artists
            .Where(x => x.Nick.Contains(request.Query));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == _currentUserId || x.ArtistsUsers.Any(su => su.UserId == _currentUserId))
            : query.Where(x => x.IsPublic);

        return await query
            .Include(x => x.Songs.Where(x =>
                x.OwnerId == _currentUserId || x.SongsUsers.Any(su => su.UserId == _currentUserId)))
            .ThenInclude(x => x.Language)
            .Select(a => a.ToModel())
            .ToPaginatedResultAsync(request);
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType)
    {
        bool isPublicContent = contentType == ContentType.Public;

        var artist = await _dbContext.Artists
            .Where(x => x.Id == artistId)
            .Where(x => isPublicContent
                ? x.IsPublic
                : x.OwnerId == _currentUserId || x.ArtistsUsers.Any(su => su.UserId == _currentUserId))
            .Include(x => x.Songs.Where(x => isPublicContent
                ? x.IsPublic
                : x.OwnerId == _currentUserId || x.SongsUsers.Any(s => s.UserId == _currentUserId)))
            .ThenInclude(x => x.Language)
            .FirstOrDefaultAsync();

        if (artist is null)
            throw new EntryNotFoundException($"Artist with Id '{artistId}' not found or you don't have access");

        return artist.ToModel();
    }

    public async Task CreateArtistAsync(ArtistCreateModel createModel)
    {
        if (await _dbContext.Artists.AnyAsync(x =>
            (!x.IsPublic && x.OwnerId == _currentUserId && x.Nick == createModel.Nick) ||
            (x.ArtistsUsers.Any(x => x.UserId == _currentUserId) && x.Nick == createModel.Nick)))
            throw new DuplicateEntryException();

        Artist artist = new()
        {
            Nick = createModel.Nick,
            OwnerId = _currentUserId,
            IsPublic = createModel.IsPublic
        };

        _dbContext.Add(artist);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteArtistAsync(int artistId)
    {
        var artist = await _dbContext.Artists
            .Include(x => x.Songs)
            .FirstOrDefaultAsync(x => x.Id == artistId);

        if (artist is null || artist.OwnerId != _currentUserId || artist.Songs.Any(x => x.IsPublic) || artist.IsPublic)
            throw new EntryNotFoundException();

        _dbContext.Artists.Remove(artist);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateArtistAsync(int id, ArtistUpdateModel updateModel)
    {
        var artist = await _dbContext.Artists
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == _currentUserId);

        if (artist is null || await _dbContext.Artists.AnyAsync(x => x.Nick == updateModel.Nick && x.Id != id))
            throw new EntryNotFoundException();

        artist.Nick = updateModel.Nick;
        await _dbContext.SaveChangesAsync();
    }

    public async Task MakeArtistPublicAsync(int artistId)
    {
        var artist = await _dbContext.Artists
            .FirstOrDefaultAsync(x => x.Id == artistId && x.OwnerId == _currentUserId);

        if (artist is null)
            throw new EntryNotFoundException();

        artist.IsPublic = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task GiveAccessToUserAsync(int artistId, int targetId)
    {
        var artist = await _dbContext.Artists
            .Include(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .FirstOrDefaultAsync(x => x.Id == artistId);

        if (artist is null || artist.ArtistsUsers.Count != 0 ||
            (_currentUserId != artist.OwnerId && targetId != _currentUserId))
            throw new EntryNotFoundException();

        _dbContext.ArtistsUsers.Add(new ArtistsUsers
        {
            ArtistId = artistId,
            UserId = targetId
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int artistId, int targetId)
    {
        var artistUser = await _dbContext.ArtistsUsers
            .Include(x => x.Artist)
            .Include(x => x.User)
                .ThenInclude(x => x.Songs.Where(x => x.ArtistId == artistId))
            .FirstOrDefaultAsync(x => x.ArtistId == artistId && x.UserId == targetId);

        if (artistUser is null ||
            (_currentUserId != artistUser.Artist.OwnerId && targetId != _currentUserId) ||
            artistUser.User.Songs.Count != 0)
            throw new EntryNotFoundException();

        _dbContext.ArtistsUsers.Remove(artistUser);
        await _dbContext.SaveChangesAsync();
    }
}