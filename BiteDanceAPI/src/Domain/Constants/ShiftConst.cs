namespace BiteDanceAPI.Domain.Constants;

public static class ShiftConst
{
    public static readonly TimeSpan Shift1Start = new(7, 0, 0);
    public static readonly TimeSpan Shift1End = new(13, 59, 0);
    public static readonly TimeSpan Shift2Start = new(14, 0, 0);
    public static readonly TimeSpan Shift2End = new(21, 59, 0);
    public static readonly TimeSpan Shift3Start = new(22, 0, 0);
    public static readonly TimeSpan Shift3End = new(6, 59, 0);
    public static readonly TimeSpan ShiftHOStart = new(11, 0, 0);
    public static readonly TimeSpan ShiftHOEnd = new(13, 30, 0);
}
