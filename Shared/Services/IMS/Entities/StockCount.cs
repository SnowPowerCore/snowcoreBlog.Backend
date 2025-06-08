namespace IMS.Entities;

public class StockCount
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = default!;
    public DateTime CountDate { get; set; }
    public List<StockCountDetail> Details { get; set; } = new();
}

public class StockCountDetail
{
    public Guid ProductId { get; set; }
    public int CountedQuantity { get; set; }
}
