using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteDanceAPI.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired()
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Case-insensitive collation

        builder.HasIndex(u => u.Email).IsUnique();

        // Child
        // Assigned to many location as admin
        builder.HasMany(u => u.AssignedLocations).WithMany(l => l.Admins);
        // Has many Daily Orders
        builder
            .HasMany<DailyOrder>()
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        // Checkin many times
        builder
            .HasMany<Checkin>()
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        // Issue many Purple scan codes
        builder
            .HasMany<PurpleScanCode>()
            .WithOne(c => c.Issuer)
            .HasForeignKey(c => c.IssuerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
