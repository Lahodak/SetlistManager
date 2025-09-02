using SetlistManager.Common.Models;

namespace SetlistManager.API.Models;

public class AddSongsModel
{
    public List<SongModel> Songs { get; set; } = [];
}