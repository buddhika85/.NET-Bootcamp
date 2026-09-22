using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Api.Data.Configurations;

public class GenreEntityConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(20);
    }
}
