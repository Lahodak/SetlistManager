namespace SetlistManager.API.Data.Entities;

public class Room : Base
{
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }    
    public int HostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public List<RoomsSetlists> RoomsSetlists { get; set; }
}