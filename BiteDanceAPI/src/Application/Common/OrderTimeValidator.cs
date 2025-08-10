namespace BiteDanceAPI.Application.Common;

public static class OrderTimeValidator
{
    public static bool IsValidOrderTime(DateOnly orderDate, DateTimeOffset currentTime)
    {


        if (orderDate.DayOfWeek == DayOfWeek.Monday)
        {
            var previousFriday = orderDate.AddDays(-3);
            var fridayDeadline = new DateTime(
                previousFriday.Year,
                previousFriday.Month,
                previousFriday.Day,
                16,
                0,
                0,
                DateTimeKind.Utc
            ).AddHours(-7);


            return currentTime < fridayDeadline;
        }


        var orderDeadline = new DateTime(
            orderDate.Year,
            orderDate.Month,
            orderDate.Day,
            16,
            0,
            0,
            DateTimeKind.Utc
        )
            .AddDays(-1)
            .AddHours(-7); // Convert to GMT+7
        return currentTime < orderDeadline;
    }
}


/*Logic cũ: Người dùng có thể đặt vào các ngày thứ 7 và chủ nhật 
  Logic mới": Người dùng không được đặt vào các ngày thứ 7 và chủ nhật*/