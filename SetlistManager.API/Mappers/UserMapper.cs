using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Mappers;

public static class UserMapper
{
    public static UserModel ToModel(this User user)
    {
        return new UserModel
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Instrument = new()
            {
                Id = user.Instrument?.Id ?? 0,
                Name = user.Instrument?.Name ?? "No Instrument"
            }
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            UserName = model.Username,
            Email = model.Email,
            IsActive = true,
            InstrumentId = model.Instrument.Id
        };
    }
}