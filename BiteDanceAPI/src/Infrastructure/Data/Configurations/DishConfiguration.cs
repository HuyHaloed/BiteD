using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        // Name
        builder.Property(d => d.Name).IsRequired().UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation
        builder.HasIndex(d => d.Name); // TODO: check query pattern
    }
}
