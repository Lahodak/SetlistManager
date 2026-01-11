using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SetlistConfig : IEntityTypeConfiguration<Setlist>
{
    public void Configure(EntityTypeBuilder<Setlist> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .IsRequired();

        builder.HasMany(r => r.Rooms)
            .WithOne(s => s.Setlist)
            .HasForeignKey(r => r.SetlistId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Owner)
            .WithMany(u => u.Setlists)
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}