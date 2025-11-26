using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SongsSetlistsConfig : IEntityTypeConfiguration<SongsSetlists>
{
    public void Configure(EntityTypeBuilder<SongsSetlists> builder)
    {
        builder.Property(ss => ss.SongId)
            .IsRequired();
        builder.Property(sl => sl.SetlistId)
            .IsRequired();
    }
}