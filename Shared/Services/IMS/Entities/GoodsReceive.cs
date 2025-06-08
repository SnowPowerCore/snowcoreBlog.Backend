namespace IMS.Entities;

public class GoodsReceive
{
    public Guid Id { get; set; }
    public string ReceiveNumber { get; set; } = default!;
    public DateTime ReceiveDate { get; set; }
    public Guid PurchaseOrderId { get; set; }
}
