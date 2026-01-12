namespace SetlistManager.Common.Models;
public class SetlistModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }  
    public List<SongModel> Songs { get; set; } = [];
}