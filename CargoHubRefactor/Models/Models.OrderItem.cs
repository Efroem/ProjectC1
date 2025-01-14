using System;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; } = -1;

    [ForeignKey("Item")] // Indicates this is a foreign key referencing Items
    public string ItemId { get; set; } = string.Empty;
    public int Amount { get; set; }

    // Navigation Properties
    public Order? Order { get; set; }
    public Item? Item { get; set; }
}
