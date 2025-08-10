
//ApplicationDbContext.cs
using System.Reflection;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiteDanceAPI.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options),
        IApplicationDbContext
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<DepartmentChargeCode> DepartmentChargeCodes => Set<DepartmentChargeCode>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<MonthlyMenu> MonthlyMenus => Set<MonthlyMenu>();
    public DbSet<DailyMenu> DailyMenus => Set<DailyMenu>();
    public DbSet<ShiftMenu> ShiftMenus => Set<ShiftMenu>();
    public DbSet<DailyOrder> DailyOrders => Set<DailyOrder>();
    public DbSet<ShiftOrder> ShiftOrders => Set<ShiftOrder>();
    public DbSet<RedScanCode> RedScanCodes => Set<RedScanCode>();
    public DbSet<PurpleScanCode> PurpleScanCodes => Set<PurpleScanCode>();
    public DbSet<RedCodeRequest> RedCodeRequests => Set<RedCodeRequest>();
    public DbSet<BlueCheckin> BlueCheckins => Set<BlueCheckin>();
    public DbSet<GreenCheckin> GreenCheckins => Set<GreenCheckin>();
    public DbSet<RedCheckin> RedCheckins => Set<RedCheckin>();
    public DbSet<PurpleCheckin> PurpleCheckins => Set<PurpleCheckin>();
    public DbSet<Checkin> Checkins => Set<Checkin>();
    public DbSet<ScanLog> ScanLogs => Set<ScanLog>(); 
    public DbSet<PaymentScheme> PaymentSchemes => Set<PaymentScheme>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
