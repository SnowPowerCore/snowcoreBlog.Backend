namespace IMS.Entities;

public class Adjustment
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; } = default!;
    public DateTime AdjustmentDate { get; set; }
    public string? Reason { get; set; }
    public List<AdjustmentDetail> Details { get; set; } = new();
}

public class AdjustmentDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
