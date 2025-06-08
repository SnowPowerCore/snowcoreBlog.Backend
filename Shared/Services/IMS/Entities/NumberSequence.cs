namespace IMS.Entities;

public class NumberSequence
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = default!;
    public int CurrentNumber { get; set; }
    public string Prefix { get; set; } = default!;
    public int Length { get; set; }
}
