namespace SetlistManager.Data.Entities;

public class Language : Base
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public virtual List<Song> Songs { get; set; } = [];
}