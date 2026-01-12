namespace SetlistManager.Data.Entities;

public class Instrument : Base
{
    public string Name { get; set; } = default!;
    public virtual List<User> Users { get; set; } = [];
}
