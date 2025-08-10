using System.Globalization;
using BiteDanceAPI.Application.MonthlyMenus.Commands;
using BiteDanceAPI.Domain.Enums;
using ClosedXML.Excel;

namespace BiteDanceAPI.Application.MonthlyMenus.Queries;

public static class MonthlyMenuTemplateParser
{
    public static CreateMonthlyMenuCommand ParseTemplate(
        byte[] excelData,
        int locationId,
        int year,
        int month
    )
    {
        using var stream = new MemoryStream(excelData);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet("Monthly Menu Template");

        var shiftMenus = new List<Commands.ShiftMenuDto>();

        var rows = worksheet.RowsUsed().Skip(1); // Skip header row
        foreach (var row in rows)
        {
            var date = DateTime.ParseExact(
                row.Cell(1).GetString(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture
            );
            var shiftType = Enum.Parse<ShiftType>(row.Cell(3).GetString().Replace(" ", ""));
            var mainDishes = new List<string>
            {
                row.Cell(7).GetString(),
                row.Cell(8).GetString(),
                row.Cell(9).GetString()
            }
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
            var soups = new List<string> { row.Cell(10).GetString() }
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
            var vegetarianCombos = new List<string> { row.Cell(11).GetString() }
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
            var sideDishes = new List<string> { row.Cell(12).GetString() }
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            shiftMenus.Add(
                new Commands.ShiftMenuDto
                {
                    DayOfMonth = DateOnly.FromDateTime(date),
                    ShiftType = shiftType,
                    MainDishes = mainDishes,
                    Soups = soups,
                    VegetarianCombos = vegetarianCombos,
                    SideDishes = sideDishes
                }
            );
        }

        return new CreateMonthlyMenuCommand
        {
            LocationId = locationId,
            Year = year,
            Month = month,
            ShiftMenus = shiftMenus
        };
    }
}
