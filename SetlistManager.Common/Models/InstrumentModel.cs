using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;
public class InstrumentModel
{
    [Required]
    public string Name { get; set; } = default!;
}