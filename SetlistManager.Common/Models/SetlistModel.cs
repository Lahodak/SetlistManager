using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetlistManager.Common.Models;
public class SetlistModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<SongModel> Songs { get; set; }
}