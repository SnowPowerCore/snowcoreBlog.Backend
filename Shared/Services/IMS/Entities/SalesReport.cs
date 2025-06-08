namespace IMS.Entities;

public class SalesReport
{
    public Guid Id { get; set; }
    public DateTime ReportDate { get; set; }
    public string? Summary { get; set; }
}
