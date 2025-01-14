using System;
using System.ComponentModel.DataAnnotations;

public class ItemLine
{
    [Key]
    public int LineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ItemGroup { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ItemGroup? Group { get; set; } = null;
}
