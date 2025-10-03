using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public  class Artist : Base
{
    [Required]
    public string Nick { get; set; } = default!;
    public virtual List<Song>? Songs { get; set; }
}