namespace SetlistManager.Common.Models;

public class UserModel
{
    public string Username { get; set; } = default!;
    public List<InstrumentModel> Instruments { get; set; } = [];
}