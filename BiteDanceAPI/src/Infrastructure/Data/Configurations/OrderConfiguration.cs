// using BiteDanceAPI.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace BiteDanceAPI.Infrastructure.Data.Configurations;
//
// public class OrderConfiguration : IEntityTypeConfiguration<Order>
// {
//     public void Configure(EntityTypeBuilder<Order> builder)
//     {
//         // Child
//         // Has many Dishes
//         builder.HasMany(o => o.Dishes).WithMany();
//         // Has many Green checkins
//         builder
//             .HasMany<GreenCheckin>()
//             .WithOne(c => c.Order)
//             .HasForeignKey(c => c.OrderId)
//             .OnDelete(DeleteBehavior.Restrict);
//     }
// }
