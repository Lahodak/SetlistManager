using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class SongCreateModel
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = default!;
    [Required]
    public int ArtistId { get; set; }
    [Required]
    public int LanguageId { get; set; }
    public string TabsURL { get; set; } = default!;
    public string AudioURL { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string Tuning { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string Key { get; set; } = default!;
    [Required]
    [Range(10, 300)]
    public int? BPM { get; set; }
    [Required]
    public bool? IsPublic { get; set; }
}