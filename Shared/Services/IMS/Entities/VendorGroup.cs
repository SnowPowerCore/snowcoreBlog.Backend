namespace IMS.Entities;

public class VendorGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
