using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data;

namespace SetlistManager.API.Services;

public class UserService
{
    public readonly UserManager<User> _userManager;
    private readonly APIDbContext _dbContext;
    public UserService(UserManager<User> userManager, APIDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task UpdateUser(UserModel model)
    {
        User user = await _userManager.FindByIdAsync(model.Id.ToString());
        if (user != null)
        {
            user.UserName = model.Username;
            user.Email = model.Email;
            if (!string.IsNullOrEmpty(model.Instrument) && model.Instrument != "No Instrument")
            {
                var instrument = await _dbContext.Instruments.FirstOrDefaultAsync(i => i.Name == model.Instrument);
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
    }
}
