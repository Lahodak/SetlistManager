using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;
using System.Reflection.Emit;

namespace SetlistManager.API.Data.Configuration;

public class RoomsSetlistsConfig: IEntityTypeConfiguration<RoomsSetlists>
{
    public void Configure(EntityTypeBuilder<RoomsSetlists> builder)
    {
        builder.HasOne(s => s.Setlist)
               .WithMany(rs => rs.RoomsSetlists)
               .HasForeignKey(rs => rs.SetlistId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss => ss.Room)
               .WithMany(sl => sl.RoomsSetlists)
               .HasForeignKey(rs => rs.RoomId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}