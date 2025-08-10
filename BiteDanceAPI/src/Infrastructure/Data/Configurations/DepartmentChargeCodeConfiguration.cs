using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class DepartmentChargeCodeConfiguration : IEntityTypeConfiguration<DepartmentChargeCode>
{
    public void Configure(EntityTypeBuilder<DepartmentChargeCode> builder)
    {
        builder
            .Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired()
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation

        builder.HasIndex(u => u.Name);
        // Children
        // Has many Red Code Requests
        builder
            .HasMany<RedCodeRequest>()
            .WithOne(r => r.DepartmentChargeCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
