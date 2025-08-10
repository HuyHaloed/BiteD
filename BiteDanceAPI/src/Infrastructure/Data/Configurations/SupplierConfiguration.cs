using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        // Child
        // Assigned to many locations
        builder
            .HasMany(x => x.AssignedLocations)
            .WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Dishes
        builder
            .HasMany<Dish>()
            .WithOne(d => d.Supplier)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Monthly Menus
        builder
            .HasMany<MonthlyMenu>()
            .WithOne(m => m.Supplier)
            .HasForeignKey(m => m.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
