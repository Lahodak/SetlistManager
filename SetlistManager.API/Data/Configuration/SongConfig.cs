using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class SongConfig : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {

    }
}
