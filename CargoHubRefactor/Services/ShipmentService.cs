using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ShipmentService : IShipmentService
{
    private readonly CargoHubDbContext _context;

    public ShipmentService(CargoHubDbContext context)
    {
        _context = context;
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

        // Update ShipmentId in the Orders table for each OrderId
        foreach (var orderId in shipment.OrderIdsList)
        {
            // Directly parse the orderId to an integer
            int parsedOrderId = int.Parse(orderId);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == parsedOrderId);
            if (order != null)
            {
                order.ShipmentId = existingShipment.ShipmentId; // Update ShipmentId in the Orders table
                _context.Orders.Update(order); // Ensure EF Core tracks the update
            }
        }
        await _context.SaveChangesAsync();

        return "Shipment successfully updated.";
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
}

