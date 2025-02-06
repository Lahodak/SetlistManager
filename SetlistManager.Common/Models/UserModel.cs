namespace SetlistManager.Common.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public List<InstrumentModel> Instruments { get; set; } = [];
}