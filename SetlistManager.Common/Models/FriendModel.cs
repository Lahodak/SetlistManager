namespace SetlistManager.Common.Models;

public class FriendModel
{
    public int Id { get; set; }
    public int FriendshipId { get; set; }
    public string Username { get; set; } = default!;
    public int InitiatedById { get; set; }
    public FriendshipState State { get; set; }
}