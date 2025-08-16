using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class SetlistConfig : IEntityTypeConfiguration<Setlist>
{
    public void Configure(EntityTypeBuilder<Setlist> builder)
    {
        builder.HasMany(r => r.Rooms)
            .WithOne(s => s.Setlist)
            .OnDelete(DeleteBehavior.Restrict); // optional: delete rooms if setlist is deleted
    }
}