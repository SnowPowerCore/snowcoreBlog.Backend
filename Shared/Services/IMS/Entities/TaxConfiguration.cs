namespace IMS.Entities;

public class TaxConfiguration
{
    public Guid Id { get; set; }
    public string TaxName { get; set; } = default!;
    public decimal Rate { get; set; }
    public string? Description { get; set; }
}
