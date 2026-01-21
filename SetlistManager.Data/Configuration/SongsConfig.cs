using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class SongsConfig : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.TabsURL)
            .HasMaxLength(512);
        
        builder.Property(x => x.AudioURL)
            .HasMaxLength(512);
        
        builder.Property(x => x.Tuning)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(x => x.Key)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.BPM)
            .IsRequired();
        
        builder.Property(x => x.IsPublic)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt);
        
        builder.HasOne(s => s.Artist)
            .WithMany(a => a.Songs)
            .HasForeignKey(s => s.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Owner)
            .WithMany(u => u.Songs)
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Language)
            .WithMany(l => l.Songs)
            .HasForeignKey(s => s.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}