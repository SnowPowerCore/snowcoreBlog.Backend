namespace IMS.Entities;

public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public Guid VendorGroupId { get; set; }
    public List<PurchaseOrderDetail> Details { get; set; } = new();
}

public class PurchaseOrderDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
