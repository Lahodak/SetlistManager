namespace SetlistManager.Common.Models;
public class SetlistModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public UserViewModel Owner { get; set; } = new();
    public List<SongModel> Songs { get; set; } = [];
}