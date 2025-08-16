using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class Room : Base
{
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }    
    public int HostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public int CurrentSongId { get; set; }
    public List<RoomsSetlists> RoomsSetlists { get; set; }
    public List<User> Users { get; set; }

    public RoomModel ToModel()
    {
        List<UserModel> userModels = [];
        foreach (var user in Users)
        { 
            var x = user.ToModel();
            userModels.Add(x);
        }

        List<SetlistModel> setlistModels = [];

        RoomModel roomModel = new()
        {
            Name = Name,
            Code = Code,
            IsActive = IsActive,
            IsPublic = IsPublic,
            HostId = HostId,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            UpdatedBy = UpdatedBy,
            CurrentSong = CurrentSongId,
            Id = Id,
            Users = userModels
            
        };
        return roomModel;
    }
    public Room ToEntity(RoomModel model)
    {
        Name = model.Name;
        Code = model.Code;
        IsActive = model.IsActive;
        IsPublic = model.IsPublic;
        HostId = model.HostId;
        CreatedAt = model.CreatedAt;
        UpdatedAt = model.UpdatedAt;
        UpdatedBy = model.UpdatedBy;
        CurrentSongId = model.CurrentSong;
        Id = model.Id;

        return this;
    }
}