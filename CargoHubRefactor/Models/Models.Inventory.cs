using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Inventory
{
    public int InventoryId { get; set; }

    [ForeignKey("Item")] // Indicates this is a foreign key referencing Items
    public string ItemId { get; set; }

    public string Description { get; set; }
    public string ItemReference { get; set; }
    [NotMapped]
    [JsonPropertyName("locations")]
    public List<int>? LocationsList {get; set; }

    [JsonPropertyName("locationsString")] // not directly used name
    public string? Locations
    { 
        get => JsonSerializer.Serialize(LocationsList); 
        set
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    // If the value is null or empty, set an empty list
                    LocationsList = new List<int>();
                }
                else
                {
                    // Handle the case where the value is a JSON array (empty or not)
                    LocationsList = JsonSerializer.Deserialize<List<int>>(value) ?? new List<int>(); // Default to empty list if deserialization fails
                }
            }
            catch (JsonException ex)
            {
                // Log the exception and the value that caused the error
                Console.WriteLine($"Error deserializing Locations field: {ex.Message}");
                Console.WriteLine($"Invalid Locations value: {value}");
                LocationsList = new List<int>();  // Default to an empty list
            }
        }
    }


    public int TotalOnHand { get; set; }
    public int TotalExpected { get; set; }
    public int TotalOrdered { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Item? Item { get; set; }
}
