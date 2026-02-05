using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}