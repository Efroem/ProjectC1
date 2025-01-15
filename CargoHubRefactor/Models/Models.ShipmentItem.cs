using System;
using System.ComponentModel.DataAnnotations.Schema;

public class ShipmentItem
{
    public int ShipmentItemId { get; set; }
    public int ShipmentId { get; set; }

    [ForeignKey("Item")] // Indicates this is a foreign key referencing Items
    public string ItemId { get; set; }
    public int Amount { get; set; }
    public bool SoftDeleted { get; set; } = false;

    public Shipment Shipment { get; set; }
    public Item Item { get; set; }
}
