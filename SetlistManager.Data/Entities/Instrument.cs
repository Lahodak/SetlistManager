namespace SetlistManager.Data.Entities;

public class Instrument : BaseEntity
{
    public string Name { get; set; } = default!;
    public virtual List<User> Users { get; set; } = [];
}
