namespace IMS.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid ProductGroupId { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public Guid UnitMeasureId { get; set; }
}
