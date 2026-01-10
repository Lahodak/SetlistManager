using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class FriendshipConfig : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(f => f.Initiator)
            .WithMany(u => u.InitiatedFriendships)
            .HasForeignKey(f => f.InitiatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Reciever)
            .WithMany(u => u.ReceivedFriendships)
            .HasForeignKey(f => f.RecieverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}