using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class DailyMenuConfiguration : IEntityTypeConfiguration<DailyMenu>
{
    public void Configure(EntityTypeBuilder<DailyMenu> builder)
    {
        // Child
        // Has many Shift Menus
        builder
            .HasMany(d => d.ShiftMenus)
            .WithOne(s => s.DailyMenu)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
