//IApplicationDbContext.cs
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Location> Locations { get; }
    public DbSet<Supplier> Suppliers { get; }
    public DbSet<Department> Departments { get; }
    public DbSet<DepartmentChargeCode> DepartmentChargeCodes { get; }
    public DbSet<Dish> Dishes { get; }
    public DbSet<MonthlyMenu> MonthlyMenus { get; }
    public DbSet<DailyMenu> DailyMenus { get; }
    public DbSet<ShiftMenu> ShiftMenus { get; }
    public DbSet<DailyOrder> DailyOrders { get; }
    public DbSet<ShiftOrder> ShiftOrders { get; }
    public DbSet<RedScanCode> RedScanCodes { get; }
    public DbSet<PurpleScanCode> PurpleScanCodes { get; }
    public DbSet<RedCodeRequest> RedCodeRequests { get; }
    public DbSet<BlueCheckin> BlueCheckins { get; }
    public DbSet<ScanLog> ScanLogs { get; }
    public DbSet<GreenCheckin> GreenCheckins { get; }
    public DbSet<Checkin> Checkins { get; }
    public DbSet<RedCheckin> RedCheckins { get; }
    public DbSet<PurpleCheckin> PurpleCheckins { get; }
    public DbSet<PaymentScheme> PaymentSchemes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
