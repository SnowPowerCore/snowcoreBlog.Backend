namespace IMS.Entities;

public class SalesOrder
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public Guid ClientGroupId { get; set; }
    public List<SalesOrderDetail> Details { get; set; } = new();
}

public class SalesOrderDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
