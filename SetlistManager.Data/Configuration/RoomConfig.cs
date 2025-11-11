using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.Code)
            .IsRequired();
        builder.Property(x => x.IsActive)
            .IsRequired();
        builder.Property(x => x.IsActive)
            .IsRequired();
        builder.Property(x => x.HostId)
            .IsRequired();
        builder.Property(x => x.Users)
            .IsRequired();
    }
}