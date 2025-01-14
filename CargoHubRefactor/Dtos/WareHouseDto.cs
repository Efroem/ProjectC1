public class WarehouseDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Zip { get; set; }
    public required string City { get; set; }
    public required string Province { get; set; }
    public required string Country { get; set; }
    public required string ContactName { get; set; }
    public required string ContactPhone { get; set; }
    public required string ContactEmail { get; set; }
    public List<string>? RestrictedClassificationsList { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}