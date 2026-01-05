using Microsoft.AspNetCore.Identity;

namespace SetlistManager.Data.Entities;

public class User : IdentityUser<int> 
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? RoomId { get; set; }
    public virtual Room? Room { get; set; }
    public int? InstrumentId { get; set; }
    public virtual Instrument? Instrument { get; set; }
    public virtual List<Token>? Tokens { get; set; } = [];
    public virtual List<Friendship> InitiatedFriendships { get; set; } = [];
    public virtual List<Friendship> ReceivedFriendships { get; set; } = [];
}