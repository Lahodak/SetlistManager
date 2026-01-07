using SetlistManager.Common.Models;

namespace SetlistManager.Data.Entities;

public class Friendship : Base 
{
    public int User1Id { get; set; }
    public virtual User User1 { get; set; } = default!;
    public int User2Id { get; set; }
    public virtual User User2 { get; set; } = default!;
    public FriendshipState State { get; set; } = FriendshipState.Pending;
}