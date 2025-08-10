using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class ShiftMenuConfiguration : IEntityTypeConfiguration<ShiftMenu>
{
    public void Configure(EntityTypeBuilder<ShiftMenu> builder)
    {
        // Child
        // Has many Dishes
        builder.HasMany(s => s.Dishes).WithMany();
        // Has manu Shift Orders
        builder.HasMany<ShiftOrder>().WithOne(o => o.ShiftMenu).OnDelete(DeleteBehavior.Restrict);
    }
}
