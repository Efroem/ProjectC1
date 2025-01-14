using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequestDate { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string ReferenceExtra { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string ShippingNotes { get; set; } = string.Empty;
    public string PickingNotes { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public int? ShipTo { get; set; }
    public int? BillTo { get; set; }
    public int? ShipmentId { get; set; }
    public double TotalAmount { get; set; }
    public double TotalDiscount { get; set; }
    public double TotalTax { get; set; }
    public double TotalSurcharge { get; set; }
    [NotMapped]
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public Warehouse? Warehouse { get; set; }
}
