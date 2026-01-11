using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class ArtistsUsersConfig : IEntityTypeConfiguration<ArtistsUsers>
{
    public void Configure(EntityTypeBuilder<ArtistsUsers> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(au => au.Artist)
            .WithMany(a => a.ArtistsUsers)
            .HasForeignKey(au => au.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(au => au.User)
            .WithMany(u => u.ArtistsUsers)
            .HasForeignKey(au => au.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}