using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class FriendshipConfig : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(f => f.User1)
            .WithMany(u => u.InitiatedFriendships)
            .HasForeignKey(f => f.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.User2)
            .WithMany(u => u.ReceivedFriendships)
            .HasForeignKey(f => f.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}