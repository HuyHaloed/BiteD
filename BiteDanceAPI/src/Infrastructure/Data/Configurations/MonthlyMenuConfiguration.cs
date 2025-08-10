using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class MonthlyMenuConfiguration : IEntityTypeConfiguration<MonthlyMenu>
{
    public void Configure(EntityTypeBuilder<MonthlyMenu> builder)
    {
        // Child
        // Has many Daily Menus
        builder
            .HasMany(m => m.DailyMenus)
            .WithOne(d => d.MonthlyMenu)
            .HasForeignKey(d => d.MonthlyMenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
