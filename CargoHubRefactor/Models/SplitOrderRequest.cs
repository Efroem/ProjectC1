//SplitOrderRequest.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SplitOrderRequest
{
    public int OrderId { get; set; }
    public List<SplitOrderItem> ItemsToSplit { get; set; } = new List<SplitOrderItem>();
}
public class SplitOrderItem
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }
}