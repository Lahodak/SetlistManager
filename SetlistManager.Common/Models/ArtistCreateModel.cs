using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class ArtistCreateModel
{
    [Required]
    [MaxLength(100)]
    public string Nick { get; set; } = default!;
    public bool IsPublic { get; set; }
}