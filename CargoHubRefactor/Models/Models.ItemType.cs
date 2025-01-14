using System;
using System.ComponentModel.DataAnnotations;

public class ItemType
{
    [Key]
    public int TypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ItemLine { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ItemLine? Line { get; set; } = null;
}