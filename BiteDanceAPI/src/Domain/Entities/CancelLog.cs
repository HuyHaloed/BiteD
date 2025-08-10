namespace BiteDanceAPI.Domain.Entities;

public class CancelLog : BaseEntity
{
    public required string UserId { get; set; }
    public required int LocationId { get; set; }
    public required int ShiftOrderId { get; set; }
    public required int OrderId { get; set; }
    public required DateTimeOffset CancelTime { get; set; }
}