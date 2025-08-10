using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        // Child
        // Has many Users
        builder
            .HasMany<User>()
            .WithOne(u => u.PreferredLocation)
            .HasForeignKey(u => u.PreferredLocationId);
        // Has many Admins
        builder.HasMany(l => l.Admins).WithMany(a => a.AssignedLocations);
        // Has many Monthly Menus
        builder
            .HasMany<MonthlyMenu>()
            .WithOne(m => m.Location)
            .HasForeignKey(m => m.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Dishes
        builder
            .HasMany<Dish>()
            .WithOne(d => d.Location)
            .HasForeignKey(d => d.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Daily Orders
        builder
            .HasMany<DailyOrder>()
            .WithOne(o => o.Location)
            .HasForeignKey(o => o.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Shift Orders
        builder
            .HasMany<ShiftOrder>()
            .WithOne(o => o.Location)
            .HasForeignKey(o => o.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Red code requests
        builder
            .HasMany<RedCodeRequest>()
            .WithOne(r => r.WorkLocation)
            .HasForeignKey(r => r.WorkLocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Red scan codes
        builder
            .HasMany<RedScanCode>()
            .WithOne(r => r.Location)
            .HasForeignKey(r => r.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        // Has many Checkins
        builder
            .HasMany<Checkin>()
            .WithOne(c => c.Location)
            .HasForeignKey(c => c.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
