using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class ShiftOrderConfiguration : IEntityTypeConfiguration<ShiftOrder>
{
    public void Configure(EntityTypeBuilder<ShiftOrder> builder)
    {
        // Child
        // Has many Dishes
        builder.HasMany(o => o.Dishes).WithMany();
        // Has one GreenCheckin
        builder
            .HasOne<GreenCheckin>()
            .WithOne(c => c.ShiftOrder)
            .HasForeignKey<GreenCheckin>(c => c.ShiftOrderId);
    }
}
