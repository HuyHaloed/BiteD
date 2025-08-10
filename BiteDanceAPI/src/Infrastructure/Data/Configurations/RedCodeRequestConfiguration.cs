using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class RedCodeRequestConfiguration : IEntityTypeConfiguration<RedCodeRequest>
{
    public void Configure(EntityTypeBuilder<RedCodeRequest> builder)
    {
        builder
            .Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired()
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation

        builder
            .Property(u => u.WorkEmail)
            .HasMaxLength(100)
            .IsRequired()
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation

        builder.HasIndex(u => u.FullName);
        builder.HasIndex(u => u.WorkEmail);
        builder.HasIndex(u => u.Created).IsDescending(true);
        // Child
        // Has many Red Scan Codes
        builder
            .HasOne(x => x.RedScanCode)
            .WithOne(r => r.RedCodeRequest)
            .HasForeignKey<RedScanCode>(x => x.RedCodeRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
