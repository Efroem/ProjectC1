using System;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; } = -1;

    [ForeignKey("Item")] // Indicates this is a foreign key referencing Items
    public string ItemId { get; set; }
    public int Amount { get; set; }
    public bool SoftDeleted { get; set; } = false;

    // Navigation Properties
    public Order? Order { get; set; }
    public Item? Item { get; set; }
}
