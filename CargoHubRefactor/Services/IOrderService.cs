//INTERFACE ORDER SERVICE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<Order?> GetOrderAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersAsync();
    Task<IEnumerable<Order>> GetOrdersAsync(int limit);
    Task<IEnumerable<Order>> GetOrdersPagedAsync(int limit, int page);

    Task<double> GetOrderPriceTotalAsync(int id);
    Task<double> GetOrderWeightTotalAsync(int id);

    Task<Order> AddOrderAsync(
        int sourceId,
        DateTime orderDate,
        DateTime requestDate,
        string reference,
        string referenceExtra,
        string orderStatus,
        string notes,
        string shippingNotes,
        string pickingNotes,
        int warehouseId,
        int? shipTo,
        int? billTo,
        int? shipmentId,
        double totalAmount,
        double totalDiscount,
        double totalTax,
        double totalSurcharge,
        List<OrderItem> orderItems);

    Task<Order> UpdateOrderAsync(
        int id,
        int sourceId,
        DateTime orderDate,
        DateTime requestDate,
        string reference,
        string referenceExtra,
        string orderStatus,
        string notes,
        string shippingNotes,
        string pickingNotes,
        int warehouseId,
        int? shipTo,
        int? billTo,
        int? shipmentId,
        double totalAmount,
        double totalDiscount,
        double totalTax,
        double totalSurcharge);

    Task<bool> DeleteOrderAsync(int orderId);
    Task<bool> SoftDeleteOrderAsync(int orderId);
    Task<Dictionary<string, Dictionary<int, int>>> GetLocationsForOrderItemsAsync(int orderId);
}
