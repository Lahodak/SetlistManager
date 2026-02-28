using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class ChangeCurrentSongModel
{
    [Required]    
    public int? RoomId { get; set; }
    [Required]
    public int? CurrentSongId { get; set; }
    [Required]
    public int? NewCurrentSongId { get; set; }
    [Required]
    public int? AdminId { get; set; }
}
