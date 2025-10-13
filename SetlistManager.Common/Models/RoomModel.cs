using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class RoomModel
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string Code { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public int HostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public List<UserModel> Users { get; set; } = [];
    public SetlistModel? Setlist { get; set; }
    public int CurrentSong { get; set; }    
}