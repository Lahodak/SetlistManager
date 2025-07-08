using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class LanguageConfig : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {

    }
}
