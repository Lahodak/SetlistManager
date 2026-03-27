using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy);

        builder.Property(x => x.CurrentSongId);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasIndex(x => x.Code)           
            .IsUnique();

        builder.Property(x => x.IsActive)
            .IsRequired();
        
        builder.Property(x => x.IsActive)
            .IsRequired();
        
        builder.Property(x => x.HostId)
            .IsRequired();
        
        builder.HasMany(x => x.Users)
            .WithOne(x => x.Room)         
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}