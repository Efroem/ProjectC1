using System.Text.Json.Serialization;

public class TransferItem
{
    public int TransferItemId { get; set; }
    public int TransferId { get; set; }   // FK to Transfer
    public string? ItemId { get; set; }   // FK to Item (Uid)
    public int Amount { get; set; }

    public Transfer? Transfer { get; set; }
    public Item? Item { get; set; }
}
