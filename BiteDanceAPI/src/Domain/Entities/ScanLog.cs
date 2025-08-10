//ScanLog.cs
namespace BiteDanceAPI.Domain.Entities;

public class ScanLog : BaseEntity
{
    public required string UserId { get; set; }
    public required int LocationId { get; set; }
    public required int ShiftOrderId { get; set; }
    public required DateTimeOffset ScanTime { get; set; }
    public required string ScanCode { get; set; }
    public required string Log { get; set; }

}
