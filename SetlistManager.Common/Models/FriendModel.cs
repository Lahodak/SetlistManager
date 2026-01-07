namespace SetlistManager.Common.Models;

public class FriendModel
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public FriendshipState State { get; set; }
}