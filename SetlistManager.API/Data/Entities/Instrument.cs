namespace SetlistManager.API.Data.Entities;

public class Instrument : Base
{
    public string Name { get; set; }
    public List<User> Users { get; set; }
}
