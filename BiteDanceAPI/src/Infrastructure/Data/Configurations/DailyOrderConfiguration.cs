using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class DailyOrderConfiguration : IEntityTypeConfiguration<DailyOrder>
{
    public void Configure(EntityTypeBuilder<DailyOrder> builder)
    {
        // Child
        // Has many Shift orders
        builder
            .HasMany<ShiftOrder>(d => d.ShiftOrders)
            .WithOne(c => c.DailyOrder)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        // Checkin green QR
        builder.HasIndex(d => new
        {
            d.UserId,
            d.LocationId,
            d.Date
        });
        // Get weekly orders
        builder.HasIndex(d => new
        {
            d.UserId,
            d.Date,
            d.Status,
        });
        // Get weekly orders status
        // Create order
        builder.HasIndex(d => new
        {
            d.LocationId,
            d.UserId,
            d.Date,
            d.Status,
        });
    }
}
