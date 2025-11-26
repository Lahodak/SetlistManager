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
            var instrument = await _dbContext.Instruments.FirstOrDefaultAsync(i => i.Name == model.Instrument.Name);
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

    public async Task<UserModel> GetCurrentUserAsync(int userId)
    {
        User? user = await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .FirstAsync(u => u.Id == userId);
        return user.ToModel();
    }

    public async Task<User?> GetUserEntityByIdAsync(int userId)
    { 
        return await _dbContext.Users
            .Include(u => u.Instrument)
            .Include(u => u.Tokens)!
                .ThenInclude(t => t.Provider)
            .FirstAsync(u => u.Id == userId);
    }

    public async Task AddUserTokenAsync(int userId, AddTokenModel tokenModel)
    {
        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = userId,
            Provider = await _dbContext.Providers
            .FirstAsync(x => x.Name == tokenModel.Provider.ToString()),
            AccessToken = tokenModel.AccessToken,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
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
}