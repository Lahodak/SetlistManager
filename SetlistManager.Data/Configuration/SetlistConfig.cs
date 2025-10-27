using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SetlistConfig : IEntityTypeConfiguration<Setlist>
{
    public void Configure(EntityTypeBuilder<Setlist> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.CreatorId)
            .IsRequired();
        builder.Property(x => x.Creator)
            .IsRequired();
        builder.Property(x => x.SongsSetlists)
            .IsRequired();
        builder.HasMany(r => r.Rooms)
            .WithOne(s => s.Setlist)
            .OnDelete(DeleteBehavior.Restrict);
    }
}