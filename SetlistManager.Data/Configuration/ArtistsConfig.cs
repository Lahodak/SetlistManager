using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class ArtistsConfig : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nick)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(o => o.Owner)
            .WithMany(a => a.Artists)
            .HasForeignKey(o => o.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}