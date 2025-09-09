namespace SetlistManager.Common.Models;

public class ChangeCurrentSongModel
{
    public int RoomId { get; set; }
    public int CurrentSongId { get; set; }
    public int NewCurrentSongId { get; set; }
    public int AdminId { get; set; }
}
