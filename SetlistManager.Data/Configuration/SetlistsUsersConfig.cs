using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SetlistsUsersConfig : IEntityTypeConfiguration<SetlistsUsers>
{
    public void Configure(EntityTypeBuilder<SetlistsUsers> builder)
    {
        builder.HasOne(su => su.Setlist)
            .WithMany(s => s.SetlistsUsers)
            .HasForeignKey(su => su.SetlistId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(su => su.User)
            .WithMany(u => u.SetlistsUsers)
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}