using System;
using System.ComponentModel.DataAnnotations;

public class ItemGroup
{
    [Key]
    public int GroupId { get; set; } // Explicitly defining the primary key
    public string Name { get; set; }
    public string Description { get; set; }
    public bool SoftDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

