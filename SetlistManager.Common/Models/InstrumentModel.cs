using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;
public class InstrumentModel
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = default!;
}