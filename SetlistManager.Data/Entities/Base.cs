using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public abstract class Base
{
    [Key]
    public int Id { get; set; }
}