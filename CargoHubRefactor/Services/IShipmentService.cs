using System.Collections.Generic;
using System.Threading.Tasks;

public interface IShipmentService
{
    Task<List<Shipment>> GetAllShipmentsAsync();
    Task<Shipment> GetShipmentByIdAsync(int id);
    Task<(string message, Shipment? shipment)> AddShipmentAsync(Shipment shipment);
    Task<string> UpdateShipmentAsync(int id, Shipment shipment);
    Task<string> DeleteShipmentAsync(int id);

    // New method to get items in a shipment
    Task<List<ShipmentItem>> GetShipmentItemsAsync(int shipmentId);
}
