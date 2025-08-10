using BiteDanceAPI.Domain.Constants;

namespace BiteDanceAPI.Domain.Enums;

public enum ShiftType
{
    Shift1,
    Shift2,
    Shift3
}

public static class ShiftTypeExtensions
{
    public static ShiftType? GetShift(TimeSpan currentTime)
    {
        if (currentTime >= ShiftConst.Shift1Start && currentTime <= ShiftConst.Shift1End)
        {
            return ShiftType.Shift1;
        }
        if (currentTime >= ShiftConst.Shift2Start && currentTime <= ShiftConst.Shift2End)
        {
            return ShiftType.Shift2;
        }
        if (currentTime >= ShiftConst.Shift3Start || currentTime <= ShiftConst.Shift3End)
        {
            return ShiftType.Shift3;
        }
        return null;
    }
     public static ShiftType? GetShiftHeadOffice(TimeSpan currentTime)
    {
        if (currentTime >= ShiftConst.ShiftHOStart && currentTime <= ShiftConst.ShiftHOEnd)
        {
            return ShiftType.Shift2;
        }
        return null;
    }
    
}


