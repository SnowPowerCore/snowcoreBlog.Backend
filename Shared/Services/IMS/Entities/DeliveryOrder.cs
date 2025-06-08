namespace IMS.Entities;

public class DeliveryOrder
{
    public Guid Id { get; set; }
    public string DeliveryNumber { get; set; } = default!;
    public DateTime DeliveryDate { get; set; }
    public Guid SalesOrderId { get; set; }
}
