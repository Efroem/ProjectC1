using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TransferService : ITransferService
{
    private readonly CargoHubDbContext _context;

    public TransferService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<(string message, Transfer? transfer)> AddTransferAsync(Transfer transfer)
    {
        // Retrieve the source location
        var sourceLocation = await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);

        if (sourceLocation == null)
        {
            return ($"Source location with ID {transfer.TransferFrom} not found.", null);
        }

        // Retrieve the destination location
        var destinationLocation = await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationId == transfer.TransferTo);

        if (destinationLocation == null)
        {
            return ($"Destination location with ID {transfer.TransferTo} not found.", null);
        }

        // Ensure both locations belong to the same warehouse
        if (sourceLocation.WarehouseId != destinationLocation.WarehouseId)
        {
            return ("Transfers must occur within the same warehouse.", null);
        }

        foreach (var item in transfer.Items)
        {
            // Check if the source location has enough stock
            if (!sourceLocation.ItemAmounts.ContainsKey(item.ItemId) ||
                sourceLocation.ItemAmounts[item.ItemId] < item.Amount)
            {
                return ($"Insufficient stock for item {item.ItemId}.", null);
            }

            // Check if the item dimensions are compatible with the destination location
            var itemDetails = await _context.Items.FindAsync(item.ItemId);
            if (itemDetails == null)
            {
                return ($"Item {item.ItemId} not found.", null);
            }

            if (itemDetails.Weight > destinationLocation.MaxWeight ||
                itemDetails.Height > destinationLocation.MaxHeight ||
                itemDetails.Width > destinationLocation.MaxWidth ||
                itemDetails.Depth > destinationLocation.MaxDepth)
            {
                return ($"Item {item.ItemId} exceeds the dimension constraints of the destination location.", null);
            }

            // Update source location ItemAmounts
            sourceLocation.ItemAmounts[item.ItemId] -= item.Amount;
            if (sourceLocation.ItemAmounts[item.ItemId] <= 0)
            {
                sourceLocation.ItemAmounts.Remove(item.ItemId);
            }

            // Update destination location ItemAmounts
            if (destinationLocation.ItemAmounts.ContainsKey(item.ItemId))
            {
                destinationLocation.ItemAmounts[item.ItemId] += item.Amount;
            }
            else
            {
                destinationLocation.ItemAmounts[item.ItemId] = item.Amount;
            }

            // Update inventories
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

            if (inventory == null)
            {
                return ($"Inventory not found for item {item.ItemId}.", null);
            }

            var locationList = inventory.Locations.Split(',').Select(int.Parse).ToList();

            // Remove source location if all items are transferred
            if (sourceLocation.ItemAmounts.ContainsKey(item.ItemId) == false)
            {
                locationList.Remove(sourceLocation.LocationId);
            }

            // Add destination location if not already in the list
            if (!locationList.Contains(destinationLocation.LocationId))
            {
                locationList.Add(destinationLocation.LocationId);
            }

            inventory.Locations = string.Join(",", locationList);
        }

        // Add the transfer to the database
        transfer.TransferStatus = "InProgress"; // Default status
        _context.Transfers.Add(transfer);

        // Save changes to locations and inventory
        _context.Locations.Update(sourceLocation);
        _context.Locations.Update(destinationLocation);
        await _context.SaveChangesAsync();

        return ("Transfer created successfully.", transfer);
    }

    public async Task<string> UpdateTransferStatusAsync(int transferId, string status)
    {
        var transfer = await _context.Transfers
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.TransferId == transferId);

        if (transfer == null)
        {
            return "Transfer not found.";
        }

        if (status == "Completed")
        {
            foreach (var item in transfer.Items)
            {
                var destinationLocation = await _context.Locations.FindAsync(transfer.TransferTo);

                if (destinationLocation == null)
                {
                    return "Destination location not found.";
                }

                // Finalize the transfer by ensuring items are correctly moved
                if (!destinationLocation.ItemAmounts.ContainsKey(item.ItemId))
                {
                    destinationLocation.ItemAmounts[item.ItemId] = 0;
                }

                destinationLocation.ItemAmounts[item.ItemId] += item.Amount;
                _context.Locations.Update(destinationLocation);
            }
        }

        transfer.TransferStatus = status;
        await _context.SaveChangesAsync();
        return "Transfer status updated successfully.";
    }

    public async Task<string> DeleteTransferAsync(int transferId)
    {
        var transfer = await _context.Transfers.FindAsync(transferId);
        if (transfer == null)
        {
            return "Transfer not found.";
        }

        _context.Transfers.Remove(transfer);
        await _context.SaveChangesAsync();
        return "Transfer deleted successfully.";
    }

    public async Task<List<Transfer>> GetAllTransfersAsync()
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .ToListAsync();
    }

    public async Task<Transfer?> GetTransferByIdAsync(int transferId)
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.TransferId == transferId);
    }
}
 
