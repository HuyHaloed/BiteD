using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder
            .Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired()
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation

        builder.HasIndex(u => u.Name);

        // Child
        // Has many Charge Codes
        builder
            .HasMany<DepartmentChargeCode>(d => d.ChargeCodes)
            .WithOne(c => c.Department)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Red Code Requests
        builder
            .HasMany<RedCodeRequest>()
            .WithOne(r => r.Department)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
