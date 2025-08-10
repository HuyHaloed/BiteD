namespace BiteDanceAPI.Domain.Events;

public class SupplierDeactivatedEvent(Supplier supplier) : BaseEvent
{
    public Supplier Supplier { get; } = supplier;
}
