namespace IMS.Entities;

public class PurchaseReturn
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = default!;
    public DateTime ReturnDate { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public List<PurchaseReturnDetail> Details { get; set; } = new();
}

public class PurchaseReturnDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}
