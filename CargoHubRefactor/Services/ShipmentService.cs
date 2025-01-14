using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ShipmentService : IShipmentService
{
    private readonly CargoHubDbContext _context;
    private readonly IOrderService _orderService;

    public ShipmentService(CargoHubDbContext context, IOrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    public async Task<List<Shipment>> GetAllShipmentsAsync()
    {
        return await _context.Shipments.Include(s => s.SourceWarehouse).ToListAsync();
    }

    public async Task<Shipment> GetShipmentByIdAsync(int id)
    {
        return await _context.Shipments
            .Include(s => s.SourceWarehouse)
            .FirstOrDefaultAsync(s => s.ShipmentId == id);
    }

    public async Task<(string message, Shipment? shipment)> AddShipmentAsync(Shipment shipment)
    {
        if (shipment == null)
        {
            return ("Error: Invalid shipment data.", null);
        }

        // Validation checks
        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;

        // Convert OrderIdsList to a comma-separated string before saving
        shipment.OrderId = string.Join(",", shipment.OrderIdsList);

        // Add shipment
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        // Update ShipmentId in the Orders table for each OrderId and create ShipmentItems
        foreach (var orderId in shipment.OrderIdsList)
        {
            if (int.TryParse(orderId, out int parsedOrderId)) // Convert string to int
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
                if (order != null)
                {
                    order.ShipmentId = shipment.ShipmentId; // Update ShipmentId in the Orders table

                    // Fetch OrderItems for this OrderId
                    var orderItems = await _context.OrderItems
                                                   .Where(oi => oi.OrderId == parsedOrderId)
                                                   .ToListAsync();

                    // Create ShipmentItems
                    foreach (var orderItem in orderItems)
                    {
                        var shipmentItem = new ShipmentItem
                        {
                            ShipmentId = shipment.ShipmentId,
                            ItemId = orderItem.ItemId,
                            Amount = orderItem.Amount
                        };
                        _context.ShipmentItems.Add(shipmentItem);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        return ("Shipment successfully created.", shipment);
    }

    public async Task<string> UpdateShipmentAsync(int id, Shipment shipment)
    {
        var existingShipment = await _context.Shipments.FindAsync(id);
        if (existingShipment == null)
        {
            return "Error: Shipment not found.";
        }

        // Update shipment fields
        existingShipment.OrderId = string.Join(",", shipment.OrderIdsList); // Update OrderIds as comma-separated string
        existingShipment.SourceId = shipment.SourceId;
        existingShipment.OrderDate = shipment.OrderDate;
        existingShipment.RequestDate = shipment.RequestDate;
        existingShipment.ShipmentDate = shipment.ShipmentDate;
        existingShipment.ShipmentType = shipment.ShipmentType;
        existingShipment.ShipmentStatus = shipment.ShipmentStatus;
        existingShipment.Notes = shipment.Notes;
        existingShipment.CarrierCode = shipment.CarrierCode;
        existingShipment.CarrierDescription = shipment.CarrierDescription;
        existingShipment.ServiceCode = shipment.ServiceCode;
        existingShipment.PaymentType = shipment.PaymentType;
        existingShipment.TransferMode = shipment.TransferMode;
        existingShipment.TotalPackageCount = shipment.TotalPackageCount;
        existingShipment.TotalPackageWeight = shipment.TotalPackageWeight;
        existingShipment.UpdatedAt = DateTime.Now;

        // Ensure the shipment entity is updated
        _context.Shipments.Update(existingShipment);

        // Remove existing ShipmentItems for this shipment
        var existingShipmentItems = _context.ShipmentItems.Where(si => si.ShipmentId == id);
        _context.ShipmentItems.RemoveRange(existingShipmentItems);

        // Update ShipmentId in the Orders table for each OrderId and repopulate ShipmentItems
        foreach (var orderId in shipment.OrderIdsList)
        {
            int parsedOrderId = int.Parse(orderId);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
            if (order != null)
            {
                order.ShipmentId = existingShipment.ShipmentId; // Update ShipmentId in the Orders table
                _context.Orders.Update(order);

                // Fetch OrderItems for this OrderId
                var orderItems = await _context.OrderItems
                                               .Where(oi => oi.OrderId == parsedOrderId)
                                               .ToListAsync();

                // Create new ShipmentItems
                foreach (var orderItem in orderItems)
                {
                    var shipmentItem = new ShipmentItem
                    {
                        ShipmentId = existingShipment.ShipmentId,
                        ItemId = orderItem.ItemId,
                        Amount = orderItem.Amount
                    };
                    _context.ShipmentItems.Add(shipmentItem);
                }
            }
        }

        await _context.SaveChangesAsync();

        return "Shipment successfully updated.";
    }

    public async Task<string> UpdateShipmentStatusAsync(int shipmentId, string newStatus)
    {
        // Fetch the shipment from the database
        var shipment = await _context.Shipments
                                      .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
        {
            return "Error: Shipment not found.";
        }

        // Update shipment status
        shipment.ShipmentStatus = newStatus;
        shipment.UpdatedAt = DateTime.UtcNow;

        if (newStatus == "IN TRANSIT")
        {
            // Fetch the ShipmentItems associated with this shipment
            var shipmentItems = await _context.ShipmentItems
                                              .Where(si => si.ShipmentId == shipmentId)
                                              .Include(si => si.Item)  // Include the Item details
                                              .ToListAsync();

            foreach (var shipmentItem in shipmentItems)
            {
                var inventory = await _context.Inventories
                                               .FirstOrDefaultAsync(i => i.ItemId == shipmentItem.ItemId);

                if (inventory != null)
                {
                    if (inventory.TotalAvailable >= shipmentItem.Amount)
                    {
                        inventory.TotalAvailable -= shipmentItem.Amount; // Reduce the TotalAvailable
                        inventory.UpdatedAt = DateTime.UtcNow;
                        _context.Inventories.Update(inventory); // Update the inventory record
                    }
                    else
                    {
                        return $"Error: Insufficient inventory for item {shipmentItem.ItemId}.";
                    }
                }
                else
                {
                    return $"Error: Inventory not found for item {shipmentItem.ItemId}.";
                }
            }
        }

        // Save the updated shipment and inventory changes
        await _context.SaveChangesAsync();

        return $"Shipment {shipmentId} status successfully updated to '{newStatus}'.";
    }


    public async Task<List<ShipmentItem>> GetShipmentItemsAsync(int shipmentId)
    {
        // Fetch the items related to the shipment
        var shipmentItems = await _context.ShipmentItems
                                           .Where(si => si.ShipmentId == shipmentId)
                                           .ToListAsync();

        return shipmentItems;
    }


    public async Task<string> DeleteShipmentAsync(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment == null)
        {
            return "Error: Shipment not found.";
        }

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
        return "Shipment successfully deleted.";
    }

    public async Task<string> SplitOrderIntoShipmentsAsync(int orderId, List<SplitOrderItem> itemsToSplit)
    {
        // Fetch the order using the order service
        var order = await _orderService.GetOrderAsync(orderId); // Fetch the order using _orderService
        if (order == null)
            return "Error: Order not found.";

        // Validate items exist in the order and have sufficient quantity
        var orderItemsDict = order.OrderItems.ToDictionary(i => i.ItemId, i => i.Amount);
        foreach (var item in itemsToSplit)
        {
            if (!orderItemsDict.ContainsKey(item.ItemId) || orderItemsDict[item.ItemId] < item.Quantity)
                return $"Error: Invalid item {item.ItemId} or insufficient quantity.";
        }

        // Adjust the original order's items
        foreach (var item in itemsToSplit)
        {
            var originalItem = order.OrderItems.First(i => i.ItemId == item.ItemId);
            originalItem.Amount -= item.Quantity;
            if (originalItem.Amount == 0)
                order.OrderItems.Remove(originalItem);
        }

        // Create a new shipment
        var newShipment = new Shipment
        {
            SourceId = order.SourceId,
            OrderDate = order.OrderDate,
            RequestDate = order.RequestDate,
            ShipmentDate = DateTime.UtcNow,
            ShipmentType = "Split Shipment",
            ShipmentStatus = "Pending",
            Notes = "Created from split order",
            TotalPackageCount = itemsToSplit.Count,
            TotalPackageWeight = itemsToSplit.Sum(i =>
                i.Quantity * (order.OrderItems.FirstOrDefault(o => o.ItemId == i.ItemId)?.Item.Weight ?? 0)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderIdsList = new List<string> { orderId.ToString() }
        };

        // Save the new shipment to the database to generate the ShipmentId
        _context.Shipments.Add(newShipment);
        await _context.SaveChangesAsync(); // Save to generate ShipmentId

        // Create and add shipment items
        foreach (var item in itemsToSplit)
        {
            var shipmentItem = new ShipmentItem
            {
                ShipmentId = newShipment.ShipmentId,
                ItemId = item.ItemId,
                Amount = item.Quantity
            };
            _context.ShipmentItems.Add(shipmentItem);
        }

        // Save all changes to the database
        await _context.SaveChangesAsync();

        // Update the order with adjusted items and other required parameters
        await _orderService.UpdateOrderAsync(
            order.Id,
            order.SourceId,
            order.OrderDate,
            order.RequestDate,
            order.Reference,
            order.ReferenceExtra,
            order.OrderStatus,
            order.Notes,
            order.ShippingNotes,
            order.PickingNotes,
            order.WarehouseId,
            order.ShipTo,
            order.BillTo,
            order.ShipmentId,
            order.TotalAmount,
            order.TotalDiscount,
            order.TotalTax,
            order.TotalSurcharge
        );

        return $"Successfully split order {orderId} into a new shipment with Shipment ID {newShipment.ShipmentId}.";
    }



    // Other methods remain unchanged
}



