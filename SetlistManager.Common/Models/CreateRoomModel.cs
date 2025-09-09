namespace SetlistManager.Common.Models;

public class CreateRoomModel
{
    public string Name { get; set; } = default!;
    public SetlistModel SetlistModel { get; set; }
    public bool IsPublic { get; set; }
}