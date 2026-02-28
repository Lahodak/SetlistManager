using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class ArtistUpdateModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Nick { get; set; } = default!;
}