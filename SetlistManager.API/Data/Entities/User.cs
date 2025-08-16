
using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class User : Base
{
    public string Username { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? RoomId { get; set; }
    public Room? Room { get; set; }
    public int InstrumentId { get; set; }
    public Instrument Instruments { get; set; }

    public UserModel ToModel()
    {
        return new();
    }
    public User ToEntity(UserModel model)
    {
        Username = model.Username;
        IsActive = true;
        RoomId = null;
        Room = null;
        
        return new();
    }
}