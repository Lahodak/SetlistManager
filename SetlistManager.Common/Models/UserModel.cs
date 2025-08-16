namespace SetlistManager.Common.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public InstrumentModel Instrument { get; set; }
    public int generalId { get; set; } = default!;
}