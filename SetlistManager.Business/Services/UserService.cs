using SetlistManager.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Data;

namespace SetlistManager.Business.Services;

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
            .FirstAsync(u => u.Id == userId);
        return user.ToModel();
    }   
}