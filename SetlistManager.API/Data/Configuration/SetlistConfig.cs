using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class SetlistConfig : IEntityTypeConfiguration<Setlist>
{
    public void Configure(EntityTypeBuilder<Setlist> builder)
    {
        builder.HasMany(s => s.RoomsSetlists)
        .WithOne(rs => rs.Setlist)
        .HasForeignKey(rs => rs.SetlistId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}