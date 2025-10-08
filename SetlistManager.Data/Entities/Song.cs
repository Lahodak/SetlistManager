using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public class Song : Base
{
    [Required]
    public string Name { get; set; } = default!;
    public virtual required Artist Artist { get; set; }
    public string TabsURL { get; set; } = default!;
    public string AudioURL { get; set; } = default!;
    public string Tuning { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int BPM { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public int LanguageId { get; set; }
    public virtual required Language Language { get; set; }
    public virtual List<SongsSetlists>? SongsSetlists { get; set; } 
}