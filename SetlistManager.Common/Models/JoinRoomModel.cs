using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class JoinRoomModel
{
    [Required]
    public string RoomCode { get; set; } = default!;
}
