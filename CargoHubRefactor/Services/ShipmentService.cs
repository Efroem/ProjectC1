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
        if (shipment.OrderId <= 0)
            return ("Error: 'OrderId' must be greater than zero.", null);
        if (shipment.SourceId <= 0)
            return ("Error: 'SourceId' must be greater than zero.", null);

        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;

        // Add shipment
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        // Update ShipmentId in the Orders table
        var order = await _context.Orders.FindAsync(shipment.OrderId);
        if (order != null)
        {
            order.ShipmentId = shipment.ShipmentId; // Update ShipmentId in the Orders table
            await _context.SaveChangesAsync();
        }

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
        existingShipment.OrderId = shipment.OrderId;
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

        await _context.SaveChangesAsync();

        // Update ShipmentId in the Orders table
        var order = await _context.Orders.FindAsync(existingShipment.OrderId);
        if (order != null)
        {
            order.ShipmentId = existingShipment.ShipmentId; // Update ShipmentId in the Orders table
            await _context.SaveChangesAsync();
        }

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
