namespace SetlistManager.Common.Models;

public class SetlistCreateModel
{
    public string Name { get; set; } = string.Empty;
    public List<SetlistSongOrderItem> Songs { get; set; } = [];
}