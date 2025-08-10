using System.Globalization;

namespace BiteDanceAPI.Application.Services;

public class HolidayService : IHolidayService
{
    private static readonly ChineseLunisolarCalendar LunarCalendar = new();
    private static readonly List<DateTime> FixedPublicHolidays =
    [
        new DateTime(1, 1, 1), // New Year's Day
        new DateTime(1, 4, 30), // Reunification Day
        new DateTime(1, 5, 1), // International Workers' Day
        new DateTime(1, 9, 2) // National Day
        // Add other fixed holidays here
    ];

    public List<DateTime> GetPublicHolidays(int year)
    {
        var holidays = new List<DateTime>(
            FixedPublicHolidays.Select(h => new DateTime(year, h.Month, h.Day))
        );
        holidays.AddRange(GetTetHolidays(year));
        return holidays.OrderBy(d => d).ToList();
    }

    public bool IsPublicHoliday(DateTime date)
    {
        return FixedPublicHolidays.Any(h => h.Month == date.Month && h.Day == date.Day)
            || IsTetHoliday(date);
    }

    internal List<DateTime> GetTetHolidays(int year)
    {
        var tetHolidays = new List<DateTime>();
        var firstDayOfLunarYear = GetFirstDayOfLunarYear(year);

        // Typically, Tet holiday lasts for 5 days starting from the first day of the lunar new year
        for (int i = 0; i < 5; i++)
        {
            tetHolidays.Add(firstDayOfLunarYear.AddDays(i));
        }

        return tetHolidays;
    }

    internal bool IsTetHoliday(DateTime date)
    {
        var firstDayOfLunarYear = GetFirstDayOfLunarYear(date.Year);
        return date >= firstDayOfLunarYear && date < firstDayOfLunarYear.AddDays(5);
    }

    private DateTime GetFirstDayOfLunarYear(int year)
    {
        for (int month = 1; month <= 2; month++) // Tet usually falls in January or February
        {
            for (int day = 1; day <= 31; day++)
            {
                var date = new DateTime(year, month, day);
                if (LunarCalendar.GetYear(date) > LunarCalendar.GetYear(date.AddDays(-1)))
                {
                    return date;
                }
            }
        }
        throw new Exception("Unable to determine first day of lunar year");
    }

    public List<DateTime> GetVegetarianDays(int year, int month)
    {
        var vegetarianDays = new List<DateTime>();
        var daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            if (IsVegetarianDay(date))
            {
                vegetarianDays.Add(date);
            }
        }

        return vegetarianDays;
    }

    public bool IsVegetarianDay(DateTime date)
    {
        int lunarDay = LunarCalendar.GetDayOfMonth(date);
        int lunarMonth = LunarCalendar.GetMonth(date);

        return IsVegetarianDay(lunarDay, lunarMonth);
    }

    private bool IsVegetarianDay(int lunarDay, int lunarMonth)
    {
        return new[] { 1, 8, 14, 15, 18, 23, 24, 28, 29, 30 }.Contains(lunarDay);
        // Common vegetarian days based on lunar calendar
        // return lunarDay == 1
        //     || lunarDay == 14
        //     || lunarDay == 15
        //     || lunarDay == 23
        //     || lunarDay == 28
        //     || lunarDay == 29
        //     || lunarDay == 30
        //     || (lunarMonth == 1 && lunarDay == 9)
        //     || (lunarMonth == 2 && lunarDay == 19)
        //     || (lunarMonth == 3 && lunarDay == 3)
        //     || (lunarMonth == 4 && lunarDay == 4)
        //     || (lunarMonth == 7 && lunarDay == 7)
        //     || (lunarMonth == 7 && lunarDay == 15)
        //     || (lunarMonth == 9 && lunarDay == 9);
    }
}
