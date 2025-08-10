namespace BiteDanceAPI.Domain.Entities;

public class PaymentScheme : BaseEntity
{
    public CodeType CodeType { get; set; }
    public ShiftType Shift { get; set; }
    public int BasePrice { get; set; }
    public int DiscountFlatPrice { get; set; }
    public int DiscountPer { get; set; }
    public int FinalPrice { get; set; }
    public int FinalDiscount { get; set; }
    public bool IsActive { get; set; }

    public required Location Location { get; set; }
}
