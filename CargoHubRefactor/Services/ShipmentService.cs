//SHIPMENT SERVICE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        return await _context.Shipments.ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetShipmentsPagedAsync(int limit, int page)
    {
        return await _context.Shipments.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }

    public async Task<List<Shipment>> GetAllShipmentsAsync(int limit)
    {
        return await _context.Shipments.Take(limit).ToListAsync();
    }

    public async Task<Shipment> GetShipmentByIdAsync(int id)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(s => s.ShipmentId == id);
    }


    public async Task<(string message, Shipment? shipment)> AddShipmentAsync(Shipment shipment)
    {
    if (shipment == null)
        {
            return ("Invalid shipment data.", null);
        }

        var validStatuses = new[] { "Pending", "In Transit", "Delivered" };

        // Default ShipmentStatus to "Pending" if not provided
        shipment.ShipmentStatus ??= "Pending";

        // Validate ShipmentStatus
        if (!validStatuses.Contains(shipment.ShipmentStatus))
        {
            return ($"Invalid ShipmentStatus. Allowed values are: {string.Join(", ", validStatuses)}.", null);
        }

        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;
        shipment.ShipmentStatus ??= "Pending";
        shipment.TotalPackageWeight = 0; // Initialize

        // Convert OrderIdsList naar een string
        shipment.OrderId = string.Join(",", shipment.OrderIdsList);


        // Add shipment
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();



        //Update shipmentid in de orders database voor elke orderid en voeg shipmentitems toe
        foreach (var orderId in shipment.OrderIdsList)
        {
            if (int.TryParse(orderId, out int parsedOrderId)) // Convert string to int
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
                if (order != null)
                {
                    order.ShipmentId = shipment.ShipmentId; // update shipmentid in orders database

                    //pak de orderitems van de order
                    var orderItems = await _context.OrderItems
                                                   .Where(oi => oi.OrderId == parsedOrderId)
                                                   .ToListAsync();

                    //nieuwe shipmentitems toevoegen
                    foreach (var orderItem in orderItems)
                    {
                        var item = await _context.Items.FirstOrDefaultAsync(i => i.Uid == orderItem.ItemId);
                        if (item == null) continue;
                        var shipmentItem = new ShipmentItem
                        {
                            ShipmentId = shipment.ShipmentId,
                            ItemId = orderItem.ItemId,
                            Amount = orderItem.Amount
                        };
                        _context.ShipmentItems.Add(shipmentItem);

                        shipment.TotalPackageWeight += item.Weight * orderItem.Amount;
                        
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
            return "Shipment not found.";
        }

        //update shipment
        existingShipment.OrderId = string.Join(",", shipment.OrderIdsList); // Update orderids as comma-separated string
        existingShipment.SourceId = shipment.SourceId;
        existingShipment.OrderDate = shipment.OrderDate;
        existingShipment.RequestDate = shipment.RequestDate;
        existingShipment.ShipmentDate = shipment.ShipmentDate;
        existingShipment.ShipmentType = shipment.ShipmentType;
        existingShipment.ShipmentStatus = "Pending";
        existingShipment.Notes = shipment.Notes;
        existingShipment.CarrierCode = shipment.CarrierCode;
        existingShipment.CarrierDescription = shipment.CarrierDescription;
        existingShipment.ServiceCode = shipment.ServiceCode;
        existingShipment.PaymentType = shipment.PaymentType;
        existingShipment.TransferMode = shipment.TransferMode;
        existingShipment.TotalPackageCount = shipment.TotalPackageCount;
        existingShipment.TotalPackageWeight = 0;
        existingShipment.UpdatedAt = DateTime.Now;

        _context.Shipments.Update(existingShipment);

        //verwijder de shipmentitems van de shipment
        var existingShipmentItems = _context.ShipmentItems.Where(si => si.ShipmentId == id);
        _context.ShipmentItems.RemoveRange(existingShipmentItems);

        //Update shipmentid in de orders database voor elke orderid en voeg shipmentitems toe
        foreach (var orderId in shipment.OrderIdsList)
        {
            int parsedOrderId = int.Parse(orderId);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
            if (order != null)
            {
                order.ShipmentId = existingShipment.ShipmentId; //update shipmentid in de orders database
                _context.Orders.Update(order);

                //pak de orderitems van de order
                var orderItems = await _context.OrderItems
                                               .Where(oi => oi.OrderId == parsedOrderId)
                                               .ToListAsync();

                //nieuwe shipmentitems toevoegen
                foreach (var orderItem in orderItems)
                {

                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Uid == orderItem.ItemId);
                    if (item == null) continue;

                    var shipmentItem = new ShipmentItem
                    {
                        ShipmentId = existingShipment.ShipmentId,
                        ItemId = orderItem.ItemId,
                        Amount = orderItem.Amount
                    };
                    _context.ShipmentItems.Add(shipmentItem);

                    existingShipment.TotalPackageWeight += item.Weight * orderItem.Amount;
                }
            }
        }

        await _context.SaveChangesAsync();

        return "Shipment successfully updated.";
    }

    public async Task<string> UpdateShipmentStatusAsync(int shipmentId, string newStatus)
    {
        //shipment van de database doormiddel van de shipmentid
        var shipment = await _context.Shipments
                                      .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
        {
            return "Shipment not found.";
        }

        var validStatuses = new[] { "Pending", "In Transit", "Delivered" };
        if (!validStatuses.Contains(newStatus))
        {
            return $"Invalid status. Allowed statuses: {string.Join(", ", validStatuses)}.";
        }


        //update the shipment status in de shipment zelf
        shipment.ShipmentStatus = newStatus;
        shipment.UpdatedAt = DateTime.UtcNow;

        List<int> warehouseIds = new List<int>();
        foreach (string orderId in shipment.OrderIdsList) {
            int orderIdInt = Convert.ToInt32(orderId);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderIdInt);
            if (order == null) continue;
            warehouseIds.Add(order.WarehouseId);
        }

        if (newStatus == "In Transit")
        {
            //pak je shipmentitems van de shipment
            var shipmentItems = await _context.ShipmentItems
                                              .Where(si => si.ShipmentId == shipmentId)
                                              .Include(si => si.Item)  //item van de shipmentitems
                                              .ToListAsync();

            foreach (var shipmentItem in shipmentItems)
            {
                var inventory = await _context.Inventories
                                               .FirstOrDefaultAsync(i => i.ItemId == shipmentItem.ItemId);

                if (inventory != null)
                {
                    //zorg dat er genoeg inventory is om de shipment 
                    //in transit te zetten dit is een dubbel check dit hoor gecheckt te worden wanneer je een order aanmaakt.
                    if (inventory.TotalAvailable >= shipmentItem.Amount)
                    {
                        // Reduce TotalAvailable and adjust TotalOnHand
                        inventory.TotalAvailable -= shipmentItem.Amount; //Reduce available inventory
                        inventory.TotalOnHand -= shipmentItem.Amount;     //Decrease total on hand as well
                        inventory.UpdatedAt = DateTime.UtcNow;            //Update de datum 

                        int amountToRemove = shipmentItem.Amount;
                        List<int> locationIds = new List<int>(inventory.LocationsList);
                        foreach (int locationId in locationIds) {
                            var location = await _context.Locations.FirstOrDefaultAsync(i => i.LocationId == locationId /*&& warehouseIds.Contains(i.WarehouseId)*/);
                            if (location == null) continue;
                            if (location.ItemAmounts[shipmentItem.ItemId] < amountToRemove) {
                                int amountToSubtract = location.ItemAmounts[shipmentItem.ItemId];
                                location.ItemAmounts.Remove(shipmentItem.ItemId);
                                inventory.LocationsList.Remove(locationId);
                                _context.Inventories.Update(inventory);
                                amountToRemove -= amountToSubtract;
                            }
                            else {
                                location.ItemAmounts[shipmentItem.ItemId] -= amountToRemove;
                                amountToRemove = 0;
                            }
                            _context.Locations.Update(location);
                            // await _context.SaveChangesAsync();
                        }

                        //update in database
                        _context.Inventories.Update(inventory);
                        // await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return $"Insufficient inventory for item {shipmentItem.ItemId}.";
                    }
                }
                else
                {
                    return $"Inventory not found for item {shipmentItem.ItemId}.";
                }
                
            }
            
        foreach (var orderId in shipment.OrderIdsList)
        {
            if (int.TryParse(orderId, out int parsedOrderId))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
                if (order != null)
                {
                    await _orderService.UpdateOrderAsync(
                        order.Id,
                        order.SourceId,
                        order.OrderDate,
                        order.RequestDate,
                        order.Reference,
                        order.ReferenceExtra,
                        "InProgress",
                        order.Notes,
                        order.ShippingNotes,
                        order.PickingNotes,
                        order.WarehouseId,
                        order.ShipTo,
                        order.BillTo,
                        order.ShipmentId,
                        order.TotalDiscount,
                        order.TotalTax,
                        order.TotalSurcharge
                    );
                }
            }
        }
    }
    else if (newStatus == "Delivered")
    {
        foreach (var orderId in shipment.OrderIdsList)
        {
            if (int.TryParse(orderId, out int parsedOrderId))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
                if (order != null)
                {
                    await _orderService.UpdateOrderAsync(
                        order.Id,
                        order.SourceId,
                        order.OrderDate,
                        order.RequestDate,
                        order.Reference,
                        order.ReferenceExtra,
                        "Delivered",
                        order.Notes,
                        order.ShippingNotes,
                        order.PickingNotes,
                        order.WarehouseId,
                        order.ShipTo,
                        order.BillTo,
                        order.ShipmentId,
                        order.TotalDiscount,
                        order.TotalTax,
                        order.TotalSurcharge
                    );
                }
            }
        }
    }
        //save
        await _context.SaveChangesAsync();

        return $"Shipment {shipmentId} status successfully updated to '{newStatus}'.";
    }



    public async Task<List<ShipmentItem>> GetShipmentItemsAsync(int shipmentId)
    {
        //pak de items van de shipment
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
            return "Shipment not found.";
        }

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
        return "Shipment successfully deleted.";
    }

    public async Task<string> SoftDeleteShipmentAsync(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment == null)
        {
            return "Shipment not found.";
        }

        shipment.SoftDeleted = true;
        await _context.SaveChangesAsync();
        return "Shipment successfully soft deleted.";
    }

    public async Task<string> SplitOrderIntoShipmentsAsync(int orderId, List<SplitOrderItem> itemsToSplit)
    {
        var order = await _orderService.GetOrderAsync(orderId);
        if (order == null)
            return "Order not found.";

        var orderItemsDict = order.OrderItems.ToDictionary(i => i.ItemId, i => i.Amount);
        foreach (var item in itemsToSplit)
        {
            if (!orderItemsDict.ContainsKey(item.ItemId) || orderItemsDict[item.ItemId] < item.Quantity)
                return $"Invalid item {item.ItemId} or insufficient quantity.";
        }

        foreach (var item in itemsToSplit)
        {
            var originalItem = order.OrderItems.First(i => i.ItemId == item.ItemId);
            originalItem.Amount -= item.Quantity;
            if (originalItem.Amount == 0)
                order.OrderItems.Remove(originalItem);
        }

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

        _context.Shipments.Add(newShipment);
        await _context.SaveChangesAsync();

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

        await _context.SaveChangesAsync();

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
            order.TotalDiscount,
            order.TotalTax,
            order.TotalSurcharge
        );

        return $"Successfully split order {orderId} into a new shipment with Shipment ID {newShipment.ShipmentId}.";
    }

}

