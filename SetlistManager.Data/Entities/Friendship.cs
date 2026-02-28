using SetlistManager.Common.Models;

namespace SetlistManager.Data.Entities;

public class Friendship : BaseEntity 
{
    public int InitiatorId { get; set; }
    public virtual User Initiator { get; set; } = default!;
    public int RecieverId { get; set; }
    public virtual User Reciever { get; set; } = default!;
    public FriendshipState State { get; set; } = FriendshipState.Pending;
}