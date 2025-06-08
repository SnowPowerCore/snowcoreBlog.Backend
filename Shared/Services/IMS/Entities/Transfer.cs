namespace IMS.Entities;

public class Transfer
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = default!;
    public DateTime TransferDate { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ToLocationId { get; set; }
    public List<TransferDetail> Details { get; set; } = new();
}

public class TransferDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
