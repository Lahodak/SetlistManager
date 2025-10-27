using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SongsSetlistsConfig : IEntityTypeConfiguration<SongsSetlists>
{
    public void Configure(EntityTypeBuilder<SongsSetlists> builder)
    {
        builder.HasOne(ss => ss.Song)
               .WithMany(s => s.SongsSetlists)
               .HasForeignKey(ss => ss.SongId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ss => ss.Setlist)
               .WithMany(sl => sl.SongsSetlists)
               .HasForeignKey(ss => ss.SetlistId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.Property(ss => ss.SongId)
            .IsRequired();
        builder.Property(ss => ss.Song)
            .IsRequired();
        builder.Property(sl => sl.SetlistId)
            .IsRequired();
        builder.Property(sl => sl.Setlist)
            .IsRequired();
    }
}