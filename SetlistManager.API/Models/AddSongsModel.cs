using SetlistManager.Common.Models;

namespace SetlistManager.API.Models;

public class AddSongsModel
{
    public List<SongUpdateModel> Songs { get; set; } = [];
}