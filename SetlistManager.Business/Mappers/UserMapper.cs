using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class UserMapper
{
    public static UserModel ToModel(this User user)
    {
        List<TokenModel>? tokens = null;

        if (user.Tokens is not null)
            tokens = user.Tokens?.Select(t => new TokenModel
            {
                AccessToken = t.AccessToken,
                RefreshToken = t.RefreshToken,
                Provider = t.Provider.Name,
            }).ToList();

        return new UserModel
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Instrument = new()
            {
                Id = user.Instrument?.Id ?? 0,
                Name = user.Instrument?.Name ?? "No Instrument"
            },
            Tokens = tokens
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            UserName = model.Username,
            Email = model.Email,
            IsActive = true,
            InstrumentId = model.Instrument?.Id
        };
    }
}