using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Api.Data.Configurations;

public class GameEntityConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Price range - 1.00 to 100.00 --> 5 numbers - 3 whole numbers + 2 decimal places
        builder.Property(x => x.Price)
            .HasPrecision(5, 2);     // decimal(5,2)
    }
}