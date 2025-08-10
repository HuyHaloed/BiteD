using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class CheckinConfiguration : IEntityTypeConfiguration<Checkin>
{
    public void Configure(EntityTypeBuilder<Checkin> builder)
    {
        // Inheritance
        builder
            .HasDiscriminator(c => c.Type)
            .HasValue<BlueCheckin>(CodeType.Blue)
            .HasValue<GreenCheckin>(CodeType.Green)
            .HasValue<RedCheckin>(CodeType.Red)
            .HasValue<PurpleCheckin>(CodeType.Purple);

        // Indexes
        // Blue checkins
        // Get weekly blue checkins
        builder.HasIndex(c => new { c.Type, c.UserId, c.Datetime });
    }
}

// public class BlueCheckinConfiguration : IEntityTypeConfiguration<BlueCheckin>
// {
//     public void Configure(EntityTypeBuilder<BlueCheckin> builder)
//     {
//         // Shared column
//         builder.Property(c => c.User).HasColumnName("User");
//         builder.Property(c => c.UserId).HasColumnName("UserId");
//     }
// }
//
// public class GreenCheckinConfiguration : IEntityTypeConfiguration<GreenCheckin>
// {
//     public void Configure(EntityTypeBuilder<GreenCheckin> builder)
//     {
//         // Shared column
//         builder.Property(c => c.User).HasColumnName("User");
//         builder.Property(c => c.UserId).HasColumnName("UserId");
//     }
// }
