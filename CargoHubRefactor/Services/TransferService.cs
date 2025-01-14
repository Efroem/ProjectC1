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
        var fromLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);
        // var fromLocation = await _context.Locations.Include(l => l.ItemAmounts).FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);
        var toLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferTo);

        if (fromLocation == null || toLocation == null)
        {
            return ("Error: Invalid source or destination location.", null);
        }

        if (fromLocation.WarehouseId != toLocation.WarehouseId)
        {
            return ("Error: Transfers must remain within the same warehouse.", null);
        }

        foreach (var item in transfer.Items)
        {
            var itemDetails = await _context.Items.FirstOrDefaultAsync(i => i.Uid == item.ItemId);
            if (itemDetails == null)
            {
                return ($"Error: Item {item.ItemId} does not exist.", null);
            }

            // Validate dimension constraints for destination location
            if (itemDetails.Weight * item.Amount > toLocation.MaxWeight ||
                itemDetails.Width > toLocation.MaxWidth ||
                itemDetails.Height > toLocation.MaxHeight ||
                itemDetails.Depth > toLocation.MaxDepth)
            {
                return ($"Error: Item {item.ItemId} exceeds destination location's constraints.", null);
            }
        }

        // Set default status to "Pending"
        transfer.TransferStatus = "Pending";
        transfer.CreatedAt = DateTime.UtcNow;
        transfer.UpdatedAt = DateTime.UtcNow;

        // Add transfer to database
        _context.Transfers.Add(transfer);

        // Save changes
        await _context.SaveChangesAsync();

        return ("Transfer successfully created.", transfer);
    }

    public async Task<string> UpdateTransferStatusAsync(int transferId, string status)
    {
        var transfer = await _context.Transfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.TransferId == transferId);
        if (transfer == null)
        {
            return "Error: Transfer not found.";
        }

        if (status == "InProgress")
        {
            var fromLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);
            if (fromLocation == null)
            {
                return "Error: Source location not found.";
            }

            foreach (var item in transfer.Items)
            {
                if (!fromLocation.ItemAmounts.ContainsKey(item.ItemId))
                {
                    return $"Error: Item {item.ItemId} does not exist in the source location.";
                }

                if (fromLocation.ItemAmounts[item.ItemId] < item.Amount)
                {
                    return $"Error: Not enough stock for item {item.ItemId} in the source location.";
                }

                // Subtract stock from source location
                fromLocation.ItemAmounts[item.ItemId] -= item.Amount;
            }
        }
        else if (status == "Completed")
        {
            var toLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferTo);
            if (toLocation == null)
            {
                return "Error: Destination location not found.";
            }

            foreach (var item in transfer.Items)
            {
                if (toLocation.ItemAmounts.ContainsKey(item.ItemId))
                {
                    toLocation.ItemAmounts[item.ItemId] += item.Amount;
                }
                else
                {
                    toLocation.ItemAmounts[item.ItemId] = item.Amount;
                }
            }
        }

        transfer.TransferStatus = status;
        transfer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return "Transfer status successfully updated.";
    }

    public async Task<List<Transfer>> GetAllTransfersAsync()
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .ThenInclude(ti => ti.Item)
            .ToListAsync();
    }

    public async Task<Transfer?> GetTransferByIdAsync(int transferId)
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .ThenInclude(ti => ti.Item)
            .FirstOrDefaultAsync(t => t.TransferId == transferId);
    }

    public async Task<string> DeleteTransferAsync(int transferId)
    {
        var transfer = await _context.Transfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.TransferId == transferId);
        if (transfer == null)
        {
            return "Error: Transfer not found.";
        }

        // Add back items to origin location if the transfer is incomplete
        if (transfer.TransferStatus != "Completed")
        {
            var fromLocation = await _context.Locations.Include(l => l.ItemAmounts).FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);

            if (fromLocation != null)
            {
                foreach (var item in transfer.Items)
                {
                    if (fromLocation.ItemAmounts.ContainsKey(item.ItemId))
                    {
                        fromLocation.ItemAmounts[item.ItemId] += item.Amount;
                    }
                }
            }
        }

        _context.Transfers.Remove(transfer);
        await _context.SaveChangesAsync();

        return "Transfer successfully deleted.";
    }
}

