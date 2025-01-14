using System;
using System.ComponentModel.DataAnnotations.Schema;


public class Shipment
{
    public int ShipmentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string ShipmentType { get; set; } = string.Empty;
    public string ShipmentStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string CarrierCode { get; set; } = string.Empty;
    public string CarrierDescription { get; set; } = string.Empty;
    public string ServiceCode { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string TransferMode { get; set; } = string.Empty;
    public int TotalPackageCount { get; set; }
    public double TotalPackageWeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Warehouse? SourceWarehouse { get; set; }

    [NotMapped] // This ensures that this property is not mapped to the database
    public List<string> OrderIdsList
    {
        get => OrderId.Split(',').ToList();
        set => OrderId = string.Join(",", value);
    }
}
