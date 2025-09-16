using SetlistManager.Common.Models;

namespace SetlistManager. Api.Models;

public class AddSongsModel
{
    public List<SongModel> Songs { get; set; } = [];
}