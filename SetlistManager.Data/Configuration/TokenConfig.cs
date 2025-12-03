using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.Data.Entities;

namespace SetlistManager.Data.Configuration;

public class TokenConfig : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.Property(x => x.ProviderId)
            .IsRequired();
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.AccessToken)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}