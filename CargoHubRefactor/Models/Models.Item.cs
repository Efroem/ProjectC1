using System;
using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item
{
    [Key]
    [Column("Uid")] // Maps the property to the original column name in the database
    public string? Uid { get; set; } // Use string instead of Guid
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string UpcCode { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string CommodityCode { get; set; } = string.Empty;
    public int ItemLine { get; set; }
    public int ItemGroup { get; set; }
    public int ItemType { get; set; }
    public int UnitPurchaseQuantity { get; set; }
    public int UnitOrderQuantity { get; set; }
    public int PackOrderQuantity { get; set; }
    public int SupplierId { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierPartNumber { get; set; } = string.Empty;
    public string Classification { get; set; } = "None";
    public double Height { get; set; } = 0;
    public double Width { get; set; } = 0;
    public double Depth { get; set; } = 0;
    public double Weight { get; set; } = 0;
    public double Price { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ItemLine? Line { get; set; } = null;
    public ItemGroup? Group { get; set; } = null;
    public ItemType? Type { get; set; } = null;
    public Supplier? Supplier { get; set; } = null;
}
