using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

public class Location
{
    public int LocationId { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    [NotMapped]
    private Dictionary<string, int>? _itemAmounts;
    [NotMapped]
    public Dictionary<string, int>? ItemAmounts
    {
        get => _itemAmounts;
        set
        {
            _itemAmounts = value;
            ItemAmountsString = JsonSerializer.Serialize(value);  // Serialize when setting the dictionary
        }
    }
    public string? ItemAmountsString
    {
        get => _itemAmounts == null ? null : JsonSerializer.Serialize(_itemAmounts);  // Return serialized JSON string
        set
        {
            try
            {
                if (string.IsNullOrEmpty(value) || value == "null")
                {
                    // If the value is null or empty, set an empty dictionary
                    _itemAmounts = new Dictionary<string, int>();
                }
                else
                {
                    // Deserialize the value into the dictionary
                    _itemAmounts = JsonSerializer.Deserialize<Dictionary<string, int>>(value) ?? new Dictionary<string, int>();  // Default to empty dictionary if deserialization fails
                }
            }
            catch (JsonException ex)
            {
                // Log the exception and the value that caused the error
                Console.WriteLine($"Error deserializing Locations field: {ex.Message}");
                Console.WriteLine($"Invalid Locations value: {value}");
                _itemAmounts = new Dictionary<string, int>();  // Default to an empty dictionary in case of failure
            }
        }
    }
    public double MaxWeight { get; set; } = 0;
    public double MaxHeight { get; set; } = 0;
    public double MaxWidth {get; set; } = 0;
    public double MaxDepth { get; set; } = 0;
    public bool IsDock { get; set; } = false;
    public bool SoftDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Warehouse Warehouse { get; set; }
}
