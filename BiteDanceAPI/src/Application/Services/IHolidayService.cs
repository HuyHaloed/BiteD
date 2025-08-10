namespace BiteDanceAPI.Application.Services;

public interface IHolidayService
{
    List<DateTime> GetPublicHolidays(int year);
    bool IsPublicHoliday(DateTime date);
    List<DateTime> GetVegetarianDays(int year, int month);
    bool IsVegetarianDay(DateTime date);
}
