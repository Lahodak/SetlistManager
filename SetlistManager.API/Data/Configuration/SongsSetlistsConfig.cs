using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class SongsSetlistsConfig : IEntityTypeConfiguration<SongsSetlists>
{
    public void Configure(EntityTypeBuilder<SongsSetlists> builder)
    {
        builder
               .HasKey(ss => new { ss.SetlistId, ss.SongId});

        builder.HasOne(ss => ss.Song)
               .WithMany(s => s.SongsSetlists)
               .HasForeignKey(ss => ss.SongId);

        builder.HasOne(ss => ss.Setlist)
               .WithMany(sl => sl.SongsSetlists)
               .HasForeignKey(ss => ss.SetlistId);
    }
}