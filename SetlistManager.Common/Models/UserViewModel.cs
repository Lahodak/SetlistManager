namespace SetlistManager.Common.Models;

public class UserViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
}