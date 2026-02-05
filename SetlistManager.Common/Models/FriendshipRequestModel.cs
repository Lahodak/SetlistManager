using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class FriendshipRequestModel
{
    [Required]
    public int? RecieverId { get; set; }
}