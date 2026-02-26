using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class RoomMapper
{
    public static RoomModel ToModel(this Room room)
    {
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
            Users = room.Users
                .Select(x => x.ToPlayerModel())
                .ToList(),
            Setlist = setlist
        };
    }
}