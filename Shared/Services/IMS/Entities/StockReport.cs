namespace IMS.Entities;

public class StockReport
{
    public Guid Id { get; set; }
    public DateTime ReportDate { get; set; }
    public string? Summary { get; set; }
}
