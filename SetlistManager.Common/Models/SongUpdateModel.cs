using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class SongUpdateModel
{
    [Required]
    [MinLength(2, ErrorMessage = "Song name must be at least 2 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Please select an artist")]
    public int? ArtistId { get; set; }

    [Required(ErrorMessage = "Please select a language")]
    public int? LanguageId { get; set; }

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
    [Range(40, 300, ErrorMessage = "BPM must be between 40 and 300")]
    public int? BPM { get; set; } = 120;

    public bool IsPublic { get; set; } = false;

    public ArtistModel? Artist { get; set; }
    public LanguageModel? Language { get; set; }
}