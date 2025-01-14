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
    public string ShipmentType { get; set; }
    public string ShipmentStatus { get; set; }
    public string Notes { get; set; }
    public string CarrierCode { get; set; }
    public string CarrierDescription { get; set; }
    public string ServiceCode { get; set; }
    public string PaymentType { get; set; }
    public string TransferMode { get; set; }
    public int TotalPackageCount { get; set; }
    public double TotalPackageWeight { get; set; }
    public bool SoftDeleted { get; set; } = false;
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
