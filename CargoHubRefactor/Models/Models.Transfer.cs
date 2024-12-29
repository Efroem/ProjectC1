public class Transfer
{
    public int TransferId { get; set; }
    public string? Reference { get; set; }
    public int TransferFrom { get; set; } // FK to the source location
    public int TransferTo { get; set; }   // FK to the target location
    public string? TransferStatus { get; set; } = "Pending"; // "Pending", "InProgress", "Completed"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<TransferItem>? Items { get; set; }
}
