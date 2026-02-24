using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extensions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class UserService : IUserService
{
    public readonly UserManager<User> _userManager;
    private readonly AppDbContext _dbContext;
    private readonly IGeniusAuthService _geniusAuthService;
    private readonly int _currentUserId;

    public UserService(UserManager<User> userManager, AppDbContext dbContext, ICurrentUserContext currentUserContext, IGeniusAuthService geniusAuthService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _geniusAuthService = geniusAuthService;
        _currentUserId = currentUserContext.UserId;
    }

    public async Task UpdateUserAsync(UserModel model)
    {
        if(model.Id != _currentUserId)
            throw new UnauthorizedAccessException();
        
        User? user = await _dbContext.Users.FindAsync(model.Id);

        if (user is null)
            throw new EntryNotFoundException();

        user.UserName = model.Username;
        user.Email = model.Email;

        if (model.Instrument is not null)
        {
            var instrument = await _dbContext.Instruments
                .FirstOrDefaultAsync(i => i.Name == model.Instrument.Name);

            if (instrument != null)
                user.Instrument = instrument;
        }
        else
        {
            user.InstrumentId = null;
            user.Instrument = null;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<UserViewModel>> GetUsersAsync(PagedRequest request)
    {
        var query = _dbContext.Users
            .Where(u =>
                string.IsNullOrEmpty(request.Query) ||
                u.UserName!.Contains(request.Query) ||
                u.Email!.Contains(request.Query))
            .Select(u => u.ToViewModel());

        return await query.ToPaginatedResultAsync(request);
    }

    public async Task<UserModel?> GetCurrentUserAsync()
    {
        var user = await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .Include(u => u.InitiatedFriendships)
                .ThenInclude(f => f.Reciever)
            .Include(u => u.ReceivedFriendships)
                .ThenInclude(f => f.Initiator)
            .FirstOrDefaultAsync(x => x.Id == _currentUserId);

        if (user is null)
            throw new EntryNotFoundException();

        return user.ToModel();
    }

    public async Task<User?> GetUserEntityByIdAsync(int userId)
    {
        return await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddGeniusTokenToUserAsync(GrantAccessTokenResultModel grantResultModel)
    {
        var user = await GetUserByTempAuthSecret(grantResultModel.State)
            ?? throw new EntryNotFoundException();

        var accessToken = await ExchangeCodeForGeniusAccessTokenAsync(grantResultModel);

        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.Name == ProviderEnum.Genius.ToString())
            ?? throw new EntryNotFoundException();

        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = user.Id,
            Provider = provider,
            AccessToken = accessToken,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    private async Task<string> ExchangeCodeForGeniusAccessTokenAsync(GrantAccessTokenResultModel grantResultModel)
    {
        var result = await _geniusAuthService.ExchangeGeniusCode(grantResultModel.Code);

        if (result?.AccessToken is null)
            throw new GeniusAccessTokenNotRecievedException();

        return result.AccessToken;
    }

    public async Task<User?> GetUserByTempAuthSecret(string salt)
    {
        var tempAuth = await _dbContext.TempAuthStorage
            .FirstOrDefaultAsync(x => x.TempSecret == salt)
            ?? throw new EntryNotFoundException();

        return await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .FirstOrDefaultAsync(u => u.Id == tempAuth.UserId);
    }

    public async Task HandleFriendshipRequestAsync(int initiatorId, FriendshipRequestModel friendshipRequest)
    {
        if (initiatorId != _currentUserId)
            throw new UnauthorizedAccessException();

        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f =>
            (f.InitiatorId == initiatorId && f.RecieverId == friendshipRequest.RecieverId) ||
            (f.InitiatorId == friendshipRequest.RecieverId && f.RecieverId == initiatorId));

        if (friendship is not null)
        {
            if (initiatorId != friendship.InitiatorId)
            {
                friendship.State = FriendshipState.Accepted;
                await _dbContext.SaveChangesAsync();
            }
            return;
        }

        _dbContext.Friendships.Add(friendshipRequest.ToEntity(initiatorId));
        await _dbContext.SaveChangesAsync();
    }

    public async Task AcceptFriendshipAsync(int id, int friendshipId)
    {
        if (_currentUserId != id)
            throw new UnauthorizedAccessException();

        var friendship = await GetFriendshipByIdAndUserIdAsync(friendshipId, _currentUserId);

        if (friendship is null || friendship.State != FriendshipState.Pending)
            return;

        friendship.State = FriendshipState.Accepted;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveFriendshipAsync(int id, int friendshipId)
    {
        if (_currentUserId != id)
            throw new UnauthorizedAccessException();

        var friendship = await GetFriendshipByIdAndUserIdAsync(friendshipId, _currentUserId)
            ?? throw new EntryNotFoundException();

        _dbContext.Friendships.Remove(friendship);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<FriendModel>> GetUserFriendsAsync(int userId, PagedRequest request)
    {
        if (userId != _currentUserId)
            throw new UnauthorizedAccessException();

        var query = _dbContext.Friendships
            .Include(f => f.Initiator)
            .Include(f => f.Reciever)
            .Where(f =>
                (f.InitiatorId == userId || f.RecieverId == userId) &&
                (string.IsNullOrEmpty(request.Query) ||
                 f.Initiator.UserName!.Contains(request.Query) ||
                 f.Reciever.UserName!.Contains(request.Query)))
            .Select(f => f.ToModel(userId));

        return await query.ToPaginatedResultAsync(request);
    }

    private async Task<Friendship?> GetFriendshipByIdAndUserIdAsync(int friendshipId, int userId)
    {
        return await _dbContext.Friendships
            .FirstOrDefaultAsync(f =>
                f.Id == friendshipId &&
                (f.InitiatorId == userId || f.RecieverId == userId));
    }
}
