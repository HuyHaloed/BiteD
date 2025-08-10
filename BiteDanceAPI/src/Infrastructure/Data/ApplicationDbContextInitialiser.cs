using BiteDanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BiteDanceAPI.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context
)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            logger.LogInformation("Try seeding database...");
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
{
    // Kiểm tra bảng Departments đã tồn tại hay chưa
    var connection = context.Database.GetDbConnection();

    await connection.OpenAsync();
    var cmd = connection.CreateCommand();
    cmd.CommandText = "SELECT OBJECT_ID(N'Departments', N'U')";
    var result = await cmd.ExecuteScalarAsync();

    if (result == DBNull.Value || result == null)
    {
        logger.LogWarning("⚠️ Table 'Departments' does not exist yet. Skipping seed.");
        return;
    }
    /*
    // Nếu bảng đã tồn tại thì mới seed
    if (!context.Departments.Any())
    {
        context.Departments.AddRange(
            [
                new Department()
                {
                    Name = "IT",
                    ChargeCodes =
                    {
                        new DepartmentChargeCode() { Name = "IT1", Code = 1111 },
                        new DepartmentChargeCode() { Name = "IT2", Code = 2222 },
                    }
                },
                new Department()
                {
                    Name = "Test",
                    ChargeCodes =
                    {
                        new DepartmentChargeCode() { Name = "Test1", Code = 3333 },
                        new DepartmentChargeCode() { Name = "Test2", Code = 3333 },
                    }
                },
                new Department()
                {
                    Name = "HR",
                    ChargeCodes =
                    {
                        new DepartmentChargeCode() { Name = "HR1", Code = 4444 },
                        new DepartmentChargeCode() { Name = "HR2", Code = 5555 },
                    }
                },
            ]
        );

        await context.SaveChangesAsync();
    }*/
}

}
