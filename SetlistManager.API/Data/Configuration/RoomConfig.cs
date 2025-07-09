using SetlistManager.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SetlistManager.API.Data.Configuration;

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasMany(s => s.RoomsSetlists)
        .WithOne(rs => rs.Room)
        .HasForeignKey(rs => rs.SetlistId)
        .OnDelete(DeleteBehavior.Restrict);

    }
}
