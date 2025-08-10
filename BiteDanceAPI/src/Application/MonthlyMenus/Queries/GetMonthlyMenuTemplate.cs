using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Services;
using ClosedXML.Excel;

namespace BiteDanceAPI.Application.MonthlyMenus.Queries;

public record GetMonthlyMenuTemplateQuery(int LocationId, int Year, int Month) : IRequest<byte[]>;

public class GetMonthlyMenuTemplateQueryHandler(
    IApplicationDbContext context,
    IHolidayService holidayService
) : IRequestHandler<GetMonthlyMenuTemplateQuery, byte[]>
{
    public async Task<byte[]> Handle(
        GetMonthlyMenuTemplateQuery request,
        CancellationToken cancellationToken
    )
    {
        var location = await context.Locations.FindOrNotFoundExceptionAsync(
            request.LocationId,
            cancellationToken
        );
        if (!location.IsActive)
        {
            throw new InvalidOperationException("Location is inactive");
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Monthly Menu Template");

        // Define headers
        var headers = new List<string>
        {
            "Date",
            "Weekday",
            "Shift Type",
            "Required",
            "Public Holiday",
            "Vegetarian Day",
            "Main Dish 1",
            "Main Dish 2",
            "Main Dish 3",
            "Soup",
            "Vegetarian Combo",
            "Side Dish"
        };

        // Add headers to worksheet
        for (int i = 0; i < headers.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Fill in the days of the month
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        int currentRow = 2;
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(request.Year, request.Month, day);
            var isWeekend =
                date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var isPublicHoliday = holidayService.IsPublicHoliday(date);
            var isVegetarianDay = holidayService.IsVegetarianDay(date);
            var isRequired =
                (isWeekend && location.EnableWeekend) || (!isWeekend && location.EnableWeekday);

            void AddRow(string shiftType)
            {
                worksheet.Cell(currentRow, 1).Value = date.ToString("yyyy-MM-dd");
                worksheet.Cell(currentRow, 2).Value = date.DayOfWeek.ToString();
                worksheet.Cell(currentRow, 3).Value = shiftType;
                worksheet.Cell(currentRow, 4).Value = isRequired ? "Yes" : "No";
                worksheet.Cell(currentRow, 5).Value = isPublicHoliday ? "Yes" : "No";
                worksheet.Cell(currentRow, 6).Value = isVegetarianDay ? "Yes" : "No";
                currentRow++;
            }

            if (location.EnableShift1)
            {
                AddRow("Shift 1");
            }
            if (location.EnableShift2)
            {
                AddRow("Shift 2");
            }
            if (location.EnableShift3)
            {
                AddRow("Shift 3");
            }
        }

        // Convert the range to a table for better formatting
        var range = worksheet.RangeUsed();
        range.CreateTable();

        // Resize columns to fit content
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
