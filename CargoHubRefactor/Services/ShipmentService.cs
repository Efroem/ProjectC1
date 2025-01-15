//SHIPMENT SERVICE
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

    public async Task<List<Shipment>> GetAllShipmentsAsync(int limit)
    {
        return await _context.Shipments.Include(s => s.SourceWarehouse).Take(limit).ToListAsync();
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

        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;

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

        //update shipment
        existingShipment.OrderId = string.Join(",", shipment.OrderIdsList); // Update orderids as comma-separated string
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
        //shipment van de database doormiddel van de shipmentid
        var shipment = await _context.Shipments
                                      .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
        {
            return "Error: Shipment not found.";
        }

        //update the shipment status in de shipment zelf
        shipment.ShipmentStatus = newStatus;
        shipment.UpdatedAt = DateTime.UtcNow;

        if (newStatus == "IN TRANSIT")
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

                        //update in database
                        _context.Inventories.Update(inventory);
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
            return "Error: Shipment not found.";
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
            return "Error: Shipment not found.";
        }

        shipment.SoftDeleted = true;
        await _context.SaveChangesAsync();
        return "Shipment successfully soft deleted.";
    }

    public async Task<string> SplitOrderIntoShipmentsAsync(int orderId, List<SplitOrderItem> itemsToSplit)
    {
        //pak de order van de database
        var order = await _orderService.GetOrderAsync(orderId); //gebruikt de method van de ordersevice
        if (order == null)
            return "Error: Order not found.";

        //kijken of de items bestaan in de order en de amount genoeg is om op the kunnen splitsen
        var orderItemsDict = order.OrderItems.ToDictionary(i => i.ItemId, i => i.Amount);
        foreach (var item in itemsToSplit)
        {
            if (!orderItemsDict.ContainsKey(item.ItemId) || orderItemsDict[item.ItemId] < item.Quantity)
                return $"Error: Invalid item {item.ItemId} or insufficient quantity.";
        }

        //order items ammount aanpassen
        foreach (var item in itemsToSplit)
        {
            var originalItem = order.OrderItems.First(i => i.ItemId == item.ItemId);
            originalItem.Amount -= item.Quantity;
            if (originalItem.Amount == 0) //als de amount van een item 0 is na het splitsen dan wordt deze verwijderd
                order.OrderItems.Remove(originalItem);
        }

        //nieuwe shipment
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

        //shipment opslaan om een id te genereren
        _context.Shipments.Add(newShipment);
        await _context.SaveChangesAsync();

        //shipment items toevoegen
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

        //save
        await _context.SaveChangesAsync();

        //Update de order
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
}

