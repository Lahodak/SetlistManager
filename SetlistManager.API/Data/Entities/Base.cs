using System.ComponentModel.DataAnnotations;

namespace SetlistManager.API.Data.Entities;
public class Base
{
    [Key]
    public int Id { get; set; }
}
