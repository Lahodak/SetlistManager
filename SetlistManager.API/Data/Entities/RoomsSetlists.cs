namespace SetlistManager.API.Data.Entities;

public class RoomsSetlists
{
    public int RoomId { get; set; }
    public Room Room { get; set; } = default!;

    public int SetlistId { get; set; }
    public Setlist Setlist { get; set; } = default!;
}
