using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SetlistManager.API.Services;

public class UserService : IUserService
{
    public readonly UserManager<User> _userManager;
    private readonly Data.AppDbContext _dbContext;

    public UserService(UserManager<User> userManager, Data.AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task UpdateUserAsync(UserModel model)
    {
        User? user = await _userManager.FindByIdAsync(model.Id.ToString());
        if (user == null)
        {
            return;
        }

        user.UserName = model.Username;
        user.Email = model.Email;
        
        if (model.Instrument is null && model.Instrument.Name != "No Instrument")
        {
            var instrument = await _dbContext.Instruments.FirstOrDefaultAsync(i => i.Name == model.Instrument.Name);
            if (instrument != null)
            {
                user.InstrumentId = instrument.Id;
                user.Instrument = instrument;
            }
        }
        else
        {
            user.InstrumentId = null;
            user.Instrument = null;
        }
        await _userManager.UpdateAsync(user);
    }

    public async Task<UserModel> GetCurrentUserAsync(int userId)
    {
        User? user = await _dbContext.Users
            .Include(u => u.Instrument)
            .FirstAsync(u => u.Id == userId);
        return user.ToModel();
    }   
}