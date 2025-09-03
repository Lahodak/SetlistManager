namespace SetlistManager.Common.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Instrument { get; set; } = default!;
}