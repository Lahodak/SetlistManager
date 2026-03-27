namespace SetlistManager.Data.Entities;

public class Provider : BaseEntity
{
    public string Name { get; set; } = default!;
    public virtual List<Token> Tokens { get; set; } = [];
}