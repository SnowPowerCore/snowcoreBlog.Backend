namespace IMS.Entities;

public class ProductGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
