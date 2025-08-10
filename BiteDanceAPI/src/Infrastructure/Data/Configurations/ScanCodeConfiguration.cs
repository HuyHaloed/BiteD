using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class ScanCodeConfiguration : IEntityTypeConfiguration<ScanCode>
{
    public void Configure(EntityTypeBuilder<ScanCode> builder)
    {
        builder.HasIndex(s => s.ValidFrom).IsDescending(true);
        // Inheritance
        builder
            .HasDiscriminator(c => c.Type)
            .HasValue<RedScanCode>(CodeType.Red)
            .HasValue<PurpleScanCode>(CodeType.Purple);

        // Child
        // TODO: move to subclass?
        // Has many Red Checkins
        builder
            .HasMany<RedCheckin>()
            .WithOne(c => c.ScanCode)
            .HasForeignKey(c => c.ScanCodeId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Purple Checkins
        builder
            .HasMany<PurpleCheckin>()
            .WithOne(c => c.ScanCode)
            .HasForeignKey(c => c.ScanCodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RedScanCodeConfiguration : IEntityTypeConfiguration<RedScanCode>
{
    public void Configure(EntityTypeBuilder<RedScanCode> builder)
    {
        builder.HasIndex(s => s.RedCodeId).IsUnique();
    }
}
