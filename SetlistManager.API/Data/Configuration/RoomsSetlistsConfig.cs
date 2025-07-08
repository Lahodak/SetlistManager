using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data.Configuration;

public class RoomsSetlistsConfig: IEntityTypeConfiguration<RoomsSetlists>
{
    public void Configure(EntityTypeBuilder<RoomsSetlists> builder)
    {

    }
}
