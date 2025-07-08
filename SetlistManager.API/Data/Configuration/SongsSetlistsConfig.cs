using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class SongsSetlistsConfig : IEntityTypeConfiguration<SongsSetlists>
{
    public void Configure(EntityTypeBuilder<SongsSetlists> builder)
    {

    }
}
