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
    public bool SoftDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}