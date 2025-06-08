namespace IMS.Entities;

public class SalesReturn
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = default!;
    public DateTime ReturnDate { get; set; }
    public Guid SalesOrderId { get; set; }
    public List<SalesReturnDetail> Details { get; set; } = new();
}

public class SalesReturnDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}
