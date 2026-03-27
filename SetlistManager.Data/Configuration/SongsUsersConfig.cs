using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SongsUsersConfig : IEntityTypeConfiguration<SongsUsers>
{
    public void Configure(EntityTypeBuilder<SongsUsers> builder)
    {
        builder.HasOne(su => su.Song)
            .WithMany(s => s.SongsUsers)
            .HasForeignKey(su => su.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(su => su.User)
            .WithMany(u => u.SongsUsers)
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    
        builder.Property(x => x.CreatedAt);
    }
}