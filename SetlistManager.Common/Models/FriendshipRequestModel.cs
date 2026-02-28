using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class FriendshipRequestModel
{
    [Required]
    public int? ReceiverId { get; set; }
}