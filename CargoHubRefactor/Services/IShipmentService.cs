//INTERFACE SHIPEMNT SERVICE
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IShipmentService
{
    Task<List<Shipment>> GetAllShipmentsAsync();
    Task<List<Shipment>> GetAllShipmentsAsync(int limit);
    Task<Shipment> GetShipmentByIdAsync(int id);
    Task<IEnumerable<Shipment>> GetShipmentsPagedAsync(int limit, int page);

    Task<(string message, Shipment? shipment)> AddShipmentAsync(Shipment shipment);
    Task<string> UpdateShipmentAsync(int id, Shipment shipment);
    Task<string> DeleteShipmentAsync(int id);
    Task<string> SoftDeleteShipmentAsync(int id);
    Task<List<ShipmentItem>> GetShipmentItemsAsync(int shipmentId);
    Task<string> SplitOrderIntoShipmentsAsync(int orderId, List<SplitOrderItem> itemsToSplit);
    Task<string> UpdateShipmentStatusAsync(int shipmentId, string newStatus);
}
