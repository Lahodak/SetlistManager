using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SetlistConfig : IEntityTypeConfiguration<Setlist>
{
    public void Configure(EntityTypeBuilder<Setlist> builder)
    {
        builder.HasMany(r => r.Rooms)
            .WithOne(s => s.Setlist)
            .OnDelete(DeleteBehavior.Restrict);
    }
}