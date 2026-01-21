namespace SetlistManager.Data.Entities;

public class Provider : Base
{
    public string Name { get; set; } = default!;
    public virtual List<Token> Tokens { get; set; } = [];
}