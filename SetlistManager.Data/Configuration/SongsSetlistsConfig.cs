using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SongsSetlistsConfig : IEntityTypeConfiguration<SongsSetlists>
{
    public void Configure(EntityTypeBuilder<SongsSetlists> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(ss => ss.Order)
            .IsRequired();

        builder.Property(ss => ss.SongId)
            .IsRequired();

        builder.Property(sl => sl.SetlistId)
            .IsRequired();
    }
}