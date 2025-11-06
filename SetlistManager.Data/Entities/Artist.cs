using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Data.Entities;

public  class Artist : Base
{
    public string Nick { get; set; } = default!;
    public virtual List<Song>? Songs { get; set; }
}