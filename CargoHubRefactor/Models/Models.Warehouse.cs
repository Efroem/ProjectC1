using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.Data.SqlClient;

public class Warehouse
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    [NotMapped]
    public List<string>? RestrictedClassificationsList { get; set; }

    public string? RestrictedClassifications
    {
        get => JsonSerializer.Serialize(RestrictedClassificationsList);
        set
        {
            try
            {
                RestrictedClassificationsList = string.IsNullOrEmpty(value)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing RestrictedClassifications field: {ex.Message}");
                RestrictedClassificationsList = new List<string>();
            }
        }
    }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}