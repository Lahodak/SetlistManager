using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;
public class Base
{
    [Key]
    public int Id { get; set; }
}
