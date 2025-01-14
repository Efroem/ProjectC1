public class LocationItem
{
    public required string ItemId { get; set; }
    public int Amount { get; set; }
    public string Classification { get; set; } = "None";

    public double Height { get; set; } = 0;
    public double Width { get; set; } = 0;
    public double Depth { get; set; } = 0;
    public double Weight { get; set; } = 0;
}
