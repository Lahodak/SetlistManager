
using Microsoft.AspNetCore.Identity;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class User : IdentityUser<int> 
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? RoomId { get; set; }
    public virtual Room? Room { get; set; }
    public int? InstrumentId { get; set; }
    public virtual Instrument? Instrument { get; set; }    
}