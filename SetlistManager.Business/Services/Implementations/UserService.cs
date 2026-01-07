using SetlistManager.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Data;

namespace SetlistManager.Business.Services.Implementations;

public class UserService : IUserService
{
    public readonly UserManager<User> _userManager;
    private readonly AppDbContext _dbContext;

    public UserService(UserManager<User> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
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

    public async Task<PagedResponse<UserModel>> GetUsersAsync(PagedRequest request)
    {
        var query = _dbContext.Users
            .Where(u => string.IsNullOrEmpty(request.Query) ||
                u.UserName!.Contains(request.Query) ||
                u.Email!.Contains(request.Query));
        
        var totalCount = await query.CountAsync();
        
        var users = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        
        PagedResponse<UserModel> pagedResponse = new()
        {
            Items = users
                .Select(u => u.ToModel())
                .ToList(),
            TotalCount = totalCount
        };

        return pagedResponse;
    }

    public async Task<UserModel?> GetCurrentUserAsync(int userId)
    {
        User? user = await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .Include(u => u.InitiatedFriendships)
                .ThenInclude(f => f.User2)
            .Include(u => u.ReceivedFriendships)
                .ThenInclude(f => f.User1)
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

    public async Task<bool> TryAddUserTokenAsync(int userId, TokenCreateModel tokenModel)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.Name == tokenModel.Provider.ToString());

        if (provider is null)
            return false;

        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = userId,
            Provider = provider,
            AccessToken = tokenModel.AccessToken,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetUserByTempSalt(string salt)
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
        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f =>
                (f.User1Id == initiatorId && f.User2Id == friendshipRequest.RecieverId) ||
                (f.User1Id == friendshipRequest.RecieverId && f.User2Id == initiatorId));

        if(friendship is not null)
        {
            friendship.State = FriendshipState.Accepted;            
            await _dbContext.SaveChangesAsync();
            
            return;
        }

        Friendship newFriendship = new()
        {
            User1Id = initiatorId,
            User2Id = friendshipRequest.RecieverId,
            State = FriendshipState.Pending
        };

        await _dbContext.Friendships.AddAsync(newFriendship);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AcceptFriendshipAsync(int currentUserId, int friendshipId)
    {
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                (f.User1Id == currentUserId || f.User2Id == currentUserId) &&
                f.State == FriendshipState.Pending);

        if (friendship is null)
            return;

        friendship.State = FriendshipState.Accepted;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveFriendshipAsync(int currentUserId, int friendshipId)
    {
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                (f.User1Id == currentUserId || f.User2Id == currentUserId));
        
        if (friendship is null)
            return;

        _dbContext.Friendships.Remove(friendship);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<FriendModel>?> GetUserFriendsAsync(int userId, PagedRequest request)
    {
        var query = _dbContext.Friendships
            .Include(f => f.User1)
            .Include(f => f.User2)
            .Where(f => (f.User1Id == userId || f.User2Id == userId) &&
                (string.IsNullOrEmpty(request.Query) ||
                f.User1.UserName!.Contains(request.Query) ||
                f.User2.UserName!.Contains(request.Query)));

        var totalCount = await query.CountAsync();

        var friendships = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        if (friendships is null || friendships.Count == 0)
            return null;

        List<FriendModel> friends = friendships.Select(f =>
        {
            var friendUser = f.User1Id == userId ? f.User2 : f.User1;
            return new FriendModel
            {
                Id = friendUser.Id,
                Username = friendUser.UserName!,
                State = f.State,
                FriendshipId = f.Id
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