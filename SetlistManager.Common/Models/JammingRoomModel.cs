namespace SetlistManager.Common.Models;

public class JammingRoomModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public List<UserModel> Users { get; set; } = [];
    public SetlistModel Setlist { get; set; }
    public int CurrentSong { get; set; }
}