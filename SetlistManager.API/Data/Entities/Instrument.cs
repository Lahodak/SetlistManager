using SetlistManager.Common.Models;

namespace SetlistManager.API.Data.Entities;

public class Instrument : Base
{
    public string Name { get; set; } = default!;
    public virtual List<User>? Users { get; set; }

    public InstrumentModel ToModel()
    {
        return new()
        {
            Name = Name
        };
    }
}
