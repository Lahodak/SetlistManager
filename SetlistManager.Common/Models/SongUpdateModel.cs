using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class SongUpdateModel
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public int? ArtistId { get; set; }
    [MaxLength(512)]
    public string TabsURL { get; set; } = default!;
    [MaxLength(512)]
    public string AudioURL { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string Tuning { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string Key { get; set; } = default!;
    [Required]
    public int? BPM { get; set; }
    [Required]
    public int? LanguageId { get; set; }
}