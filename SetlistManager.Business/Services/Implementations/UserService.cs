using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IGeniusAuthService _geniusAuthService;

    public UserService(UserManager<User> userManager, AppDbContext dbContext, ICurrentUserContext currentUserContext, IGeniusAuthService geniusAuthService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
        _geniusAuthService = geniusAuthService;
    }

    public async Task UpdateUserAsync(UserModel model)
    {
        User? user = await _dbContext.Users.FindAsync(model.Id);

        if (user is null)
            return;

        user.UserName = model.Username;
        user.Email = model.Email;

        if (model.Instrument is not null)
        {
            var instrument = await _dbContext.Instruments.
                FirstOrDefaultAsync(i => i.Name == model.Instrument.Name);
            
            if (instrument != null)
            {
                user.Instrument = instrument;
            }
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
            .Where(u => string.IsNullOrEmpty(request.Query) 
                || u.UserName!.Contains(request.Query) 
                || u.Email!.Contains(request.Query));
        
        var totalCount = await query.CountAsync();
        
        var users = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        
        PagedResponse<UserViewModel> pagedResponse = new()
        {
            Items = users
                .Select(u => u.ToViewModel())
                .ToList(),
            TotalCount = totalCount
        };

        return pagedResponse;
    }

    public async Task<UserModel?> GetCurrentUserAsync()
    {
        var userId = _currentUserContext.GetCurrentUserId()!.Value;

        User? user = await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .Include(u => u.InitiatedFriendships)
                .ThenInclude(f => f.Reciever)
            .Include(u => u.ReceivedFriendships)
                .ThenInclude(f => f.Initiator)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
            return null;

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

    private async Task<string> ExchangeCodeForGeniusAccessTokenAsync(GrantAccessTokenResultModel grantResultModel)
    {
        var resultAccessTokenModel = await _geniusAuthService.ExchangeGeniusCode(grantResultModel.Code);

        if (resultAccessTokenModel?.AccessToken is null)
            throw new GeniusAccessTokenNotRecievedException();

        return resultAccessTokenModel.AccessToken;
    }

    public async Task TryAddGeniusTokenToUserAsync(GrantAccessTokenResultModel grantResultModel)
    {
        var user = await GetUserByTempAuthSecret(grantResultModel.State)
            ?? throw new UserNotFoundException();

        TokenCreateModel tokenModel = new()
        {
            Provider = ProviderEnum.Genius,
            AccessToken = await ExchangeCodeForGeniusAccessTokenAsync(grantResultModel),
            RefreshToken = null
        };

        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.Name == tokenModel.Provider.ToString())
            ?? throw new ProviderNotFoundException(tokenModel.Provider.ToString());

        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = user.Id,
            Provider = provider,
            AccessToken = tokenModel.AccessToken,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<User?> GetUserByTempAuthSecret(string salt)
    {
        var tempAuth = await _dbContext.TempAuthStorage
            .FirstOrDefaultAsync(x => x.TempSecret == salt);

        if (tempAuth is null)
            return null;

        return await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .FirstOrDefaultAsync(u => u.Id == tempAuth.UserId);
    }

    public async Task HandleFriendshipRequestAsync(int initiatorId, FriendshipRequestModel friendshipRequest)
    {
        if(initiatorId != _currentUserContext.GetCurrentUserId()!.Value)
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
        
        Friendship newFriendship = new()
        {
            InitiatorId = initiatorId,
            RecieverId = friendshipRequest.RecieverId!.Value,
            State = FriendshipState.Pending
        };

        _dbContext.Friendships.Add(newFriendship);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<Friendship?> GetFriendshipByIdAndUserIdAsync(int friendshipId, int userId)
    {
        return await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                (f.InitiatorId == userId || f.RecieverId == userId));
    }

    public async Task AcceptFriendshipAsync(int id, int friendshipId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;
        
        if(currentUserId != id)
            throw new UnauthorizedAccessException();

        var friendship = await GetFriendshipByIdAndUserIdAsync(friendshipId, currentUserId);

        if (friendship is null || friendship.State != FriendshipState.Pending)
            return;

        friendship.State = FriendshipState.Accepted;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveFriendshipAsync(int id, int friendshipId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        if (currentUserId != id)
            throw new UnauthorizedAccessException();

        var friendship = await GetFriendshipByIdAndUserIdAsync(friendshipId, currentUserId);
        
        if (friendship is null)
            return;

        _dbContext.Friendships.Remove(friendship);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<FriendModel>?> GetUserFriendsAsync(int userId, PagedRequest request)
    {
        if(userId != _currentUserContext.GetCurrentUserId()!.Value)
            throw new UnauthorizedAccessException();

        var query = _dbContext.Friendships
            .Include(f => f.Initiator)
            .Include(f => f.Reciever)
            .Where(f => (f.InitiatorId == userId || f.RecieverId == userId) &&
                (string.IsNullOrEmpty(request.Query) ||
                f.Initiator.UserName!.Contains(request.Query) ||
                f.Reciever.UserName!.Contains(request.Query)));

        var totalCount = await query.CountAsync();

        var friendships = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        if (friendships is null || friendships.Count == 0)
            return null;

        List<FriendModel> friends = friendships.Select(f =>
        {
            var friendUser = f.InitiatorId == userId ? f.Reciever : f.Initiator;
            return new FriendModel
            {
                Id = friendUser.Id,
                Username = friendUser.UserName!,
                State = f.State,
                FriendshipId = f.Id,
                InitiatedById = f.InitiatorId
            };
        }).ToList();

        PagedResponse<FriendModel> pagedResponse = new()
        {
            Items = friends,
            TotalCount = totalCount
        };

        return pagedResponse;
    }
}