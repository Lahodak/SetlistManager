namespace SetlistManager.Common.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public InstrumentModel? Instrument { get; set; }
    public List<TokenModel>? Tokens { get; set; }
    public List<FriendModel>? Friends { get; set; }
}