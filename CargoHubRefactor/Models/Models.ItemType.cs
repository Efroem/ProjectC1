using System;
using System.ComponentModel.DataAnnotations;

public class ItemType
{
    [Key]
    public int TypeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ItemLine { get; set; }
    public bool SoftDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ItemLine? Line { get; set; } = null;
}