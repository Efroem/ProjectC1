//ORDERSERVICE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class OrderService : IOrderService
{
    private readonly CargoHubDbContext _context;

    public OrderService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(l => l.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(int limit)
    {
        return await _context.Orders.Take(limit).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersPagedAsync(int limit, int page)
    {
        return await _context.Orders.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }


    public async Task<double> GetOrderPriceTotalAsync(int id)
    {
        List<OrderItem> orderItems = await _context.OrderItems.Where(o => o.OrderId == id).ToListAsync();
        if (orderItems.IsNullOrEmpty()) return -1;

        double totalPrice = 0;

        foreach (var orderItem in orderItems)
        {
            var item = await _context.Items.FirstOrDefaultAsync(o => o.Uid == orderItem.ItemId);
            if (item == null) continue;
            double price = item.Price;
            totalPrice += price;
        }
        return totalPrice;
    }

    public async Task<double> GetOrderWeightTotalAsync(int id)
    {
        List<OrderItem> orderItems = await _context.OrderItems.Where(o => o.OrderId == id).ToListAsync();
        if (orderItems.IsNullOrEmpty()) return -1;

        double totalWeight = 0;

        foreach (var orderItem in orderItems)
        {
            var item = await _context.Items.FirstOrDefaultAsync(o => o.Uid == orderItem.ItemId);
            if (item == null) continue;
            double weight = item.Weight * orderItem.Amount;
            totalWeight += weight;
        }
        return totalWeight;
    }

    public async Task<Order> AddOrderAsync(int sourceId, DateTime orderDate, DateTime requestDate, string reference,
                                           string referenceExtra, string notes, string shippingNotes, string pickingNotes,
                                           int warehouseId, int? shipTo, int? billTo, int? shipmentId,
                                           double totalDiscount, double totalTax, double totalSurcharge, List<OrderItem> orderItems)
    {
        int nextId;

        if (_context.Orders.Any())
        {
            nextId = _context.Orders.Max(l => l.Id) + 1;
        }
        else
        {
            nextId = 1;
        }

        // Calculate total amount
        double totalAmount = 0;
        foreach (var item in orderItems)
        {
            var dbItem = await _context.Items.FirstOrDefaultAsync(i => i.Uid == item.ItemId);
            if (dbItem == null) continue;

            totalAmount += dbItem.Price * item.Amount;
        }

        var order = new Order
        {
            Id = nextId,
            SourceId = sourceId,
            OrderDate = orderDate,
            RequestDate = requestDate,
            Reference = reference,
            ReferenceExtra = referenceExtra,
            OrderStatus = "Pending", // Automatically set to "Pending"
            Notes = notes,
            ShippingNotes = shippingNotes,
            PickingNotes = pickingNotes,
            WarehouseId = warehouseId,
            ShipTo = shipTo,
            BillTo = billTo,
            ShipmentId = shipmentId,
            TotalAmount = totalAmount,
            TotalDiscount = totalDiscount,
            TotalTax = totalTax,
            TotalSurcharge = totalSurcharge,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);

        int nextOrderItemId;
        if (_context.OrderItems.Any())
        {
            nextOrderItemId = _context.OrderItems.Max(l => l.Id) + 1;
        }
        else
        {
            nextOrderItemId = 1;
        }

        foreach (OrderItem item in orderItems)
        {
            var orderItem = new OrderItem
            {
                Id = nextOrderItemId,
                OrderId = nextId,
                ItemId = item.ItemId,
                Amount = item.Amount
            };

            nextOrderItemId++;

            await _context.OrderItems.AddAsync(orderItem);
        }

        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateOrderAsync(int id, int sourceId, DateTime orderDate, DateTime requestDate,
                                              string reference, string referenceExtra, string orderStatus,
                                              string notes, string shippingNotes, string pickingNotes,
                                              int warehouseId, int? shipTo, int? billTo, int? shipmentId,
                                              double totalDiscount, double totalTax, double totalSurcharge)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return null;
        }

        // Restrict OrderStatus updates
        if (orderStatus != "InProgress" && orderStatus != "Delivered" && orderStatus != "Pending")
        {
            throw new InvalidOperationException("OrderStatus can only be updated to 'Pending', 'InProgress' or 'Delivered'.");
        }

        order.SourceId = sourceId;
        order.OrderDate = orderDate;
        order.RequestDate = requestDate;
        order.Reference = reference;
        order.ReferenceExtra = referenceExtra;
        order.OrderStatus = orderStatus; // Update status if valid
        order.Notes = notes;
        order.ShippingNotes = shippingNotes;
        order.PickingNotes = pickingNotes;
        order.WarehouseId = warehouseId;
        order.ShipTo = shipTo;
        order.BillTo = billTo;
        order.ShipmentId = shipmentId;
        order.TotalDiscount = totalDiscount;
        order.TotalTax = totalTax;
        order.TotalSurcharge = totalSurcharge;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> SoftDeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(c => c.Id == id);
        if (order == null)
        {
            return false;
        }

        order.SoftDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Dictionary<string, Dictionary<int, int>>> GetLocationsForOrderItemsAsync(int orderId)
    {
        Dictionary<string, Dictionary<int, int>> itemsWithLocations = new Dictionary<string, Dictionary<int, int>>();
        // Get the order including its items
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);


        var orderItems = await _context.OrderItems.Where(o => o.OrderId == orderId).ToListAsync();

        if (order == null || orderItems.IsNullOrEmpty())
            return null;

        // Extract item UIDs from the order
        var ItemIds = orderItems.Select(i => i.ItemId).ToList();

        foreach(string itemId in ItemIds) {
            Dictionary<int, int> locationWithAmount = new Dictionary<int, int>();
            var locations = await _context.Locations.Where(l => l.ItemAmountsString.Contains(itemId) && l.WarehouseId == order.WarehouseId).ToListAsync();
            if (locations.IsNullOrEmpty()) continue;
            itemsWithLocations.Add(itemId, new Dictionary<int, int>());
            foreach(Location location in locations) {
                itemsWithLocations[itemId].Add(location.LocationId, location.ItemAmounts[itemId]);
            }
        }
        return itemsWithLocations;
    }

}
