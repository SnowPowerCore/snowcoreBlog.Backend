namespace IMS.Entities;

public class Scrapping
{
    public Guid Id { get; set; }
    public string ScrapNumber { get; set; } = default!;
    public DateTime ScrapDate { get; set; }
    public List<ScrappingDetail> Details { get; set; } = new();
}

public class ScrappingDetail
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}
