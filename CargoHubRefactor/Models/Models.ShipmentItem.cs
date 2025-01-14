using System;
using System.ComponentModel.DataAnnotations.Schema;

public class ShipmentItem
{
    public int ShipmentItemId { get; set; }
    public int ShipmentId { get; set; }

    [ForeignKey("Item")] // Indicates this is a foreign key referencing Items
    public string ItemId { get; set; } = string.Empty;
    public int Amount { get; set; }

    public Shipment? Shipment { get; set; }
    public Item? Item { get; set; }
}
