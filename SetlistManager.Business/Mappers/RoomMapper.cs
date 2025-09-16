using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManger.Business.Mappers;

public static class RoomMapper
{
    public static RoomModel ToModel(this Room room)
    {
        List<UserModel> userModels = [];

        if (room.Users is not null)
        {
            foreach (var user in room.Users)
            {
                var x = user.ToModel();
                userModels.Add(x);
            }
        }

        SetlistModel? setlist = null;

        if (room.Setlist is not null)
            setlist = room.Setlist.ToModel();

        return new()
        {
            Name = room.Name,
            Code = room.Code,
            IsActive = room.IsActive,
            IsPublic = room.IsPublic,
            HostId = room.HostId,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
            UpdatedBy = room.UpdatedBy,
            CurrentSong = room.CurrentSongId,
            Id = room.Id,
            Users = userModels,
            Setlist = setlist
        };
    }

    public static Room ToEntity(this RoomModel model)
    {
        return new Room
        {
            Name = model.Name,
            Code = model.Name,
            IsActive = model.IsActive,
            IsPublic = model.IsPublic,
            HostId = model.HostId,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = model.UpdatedBy,
            CurrentSongId = model.CurrentSong,
            SetlistId = model.Setlist?.Id
        };
    }
}