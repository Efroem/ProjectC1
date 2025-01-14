using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.Data.SqlClient;

public class Warehouse
{
    public int WarehouseId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    [NotMapped]
    public List<string>? RestrictedClassificationsList { get; set; }

    public string? RestrictedClassifications
    { 
        get => JsonSerializer.Serialize(RestrictedClassificationsList); 
        set
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    // If the value is null or empty, set an empty list
                    RestrictedClassificationsList = new List<string>();
                }
                else
                {
                    // Handle the case where the value is a JSON array (empty or not)
                    RestrictedClassificationsList = JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>(); // Default to empty list if deserialization fails
                }
            }
            catch (JsonException ex)
            {
                // Log the exception and the value that caused the error
                Console.WriteLine($"Error deserializing Locations field: {ex.Message}");
                Console.WriteLine($"Invalid Locations value: {value}");
                RestrictedClassificationsList = new List<string>();  // Default to an empty list
            }
        }
    }
    

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
