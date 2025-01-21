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
        var toLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferTo);

        if (fromLocation == null || toLocation == null)
        {
            return ("Invalid source or destination location.", null);
        }

        if (fromLocation.WarehouseId != toLocation.WarehouseId)
        {
            return ("Transfers must remain within the same warehouse.", null);
        }

        foreach (var item in transfer.Items)
        {
            var itemDetails = await _context.Items.FirstOrDefaultAsync(i => i.Uid == item.ItemId);
            if (itemDetails == null)
            {
                return ($"Item {item.ItemId} does not exist.", null);
            }

            if (!fromLocation.ItemAmounts.ContainsKey(item.ItemId) || fromLocation.ItemAmounts[item.ItemId] < item.Amount)
            {
                return ($"Not enough stock for item {item.ItemId} in the source location.", null);
            }

            // Validate dimension constraints for destination location
            if (itemDetails.Weight * item.Amount > toLocation.MaxWeight ||
                itemDetails.Width > toLocation.MaxWidth ||
                itemDetails.Height > toLocation.MaxHeight ||
                itemDetails.Depth > toLocation.MaxDepth)
            {
                return ($"Item {item.ItemId} exceeds destination location's constraints.", null);
            }

            if (!string.IsNullOrEmpty(transfer.TransferStatus) && transfer.TransferStatus != "Pending")
            {
                return ("Invalid TransferStatus. New transfers can only have a status of 'Pending'.", null);
            }

        }
        int nextId = 1;
        if (_context.Transfers.Any())
        {
            var usedIds = await _context.Transfers.Select(t => t.TransferId).ToListAsync();
            usedIds.Sort(); // Ensure the list is sorted
            for (int i = 1; i <= usedIds.Count; i++)
            {
                if (!usedIds.Contains(i))
                {
                    nextId = i;
                    break;
                }
            }
            if (nextId == 1 && !usedIds.Contains(1))
            {
                nextId = 1;
            }
            else if (usedIds.Count == usedIds.Max())
            {
                nextId = usedIds.Max() + 1;
            }
        }

        // Set default status to "Pending"
        transfer.TransferId = nextId;
        transfer.TransferStatus = "Pending";
        transfer.CreatedAt = DateTime.UtcNow;
        transfer.UpdatedAt = DateTime.UtcNow;

        // Add transfer to database
        _context.Transfers.Add(transfer);

        // Save changes
        await _context.SaveChangesAsync();

        return ("Transfer successfully created.", transfer);
    }

    public async Task<(string message, Transfer? transfer)> UpdateTransferAsync(int transferId, Transfer updatedTransfer)
    {
        var existingTransfer = await _context.Transfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.TransferId == transferId);
        if (existingTransfer == null)
        {
            return ("Transfer not found.", null);
        }

        if (updatedTransfer.TransferStatus != "Pending")
        {
            return ("TransferStatus cannot be updated using this method. Leave the TransferStatus field on 'Pending'. To update the status, use the /status endpoint instead.", null);
        }

        // Validate `TransferFrom` and `TransferTo` locations
        var fromLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == updatedTransfer.TransferFrom);
        var toLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == updatedTransfer.TransferTo);
        if (fromLocation == null || toLocation == null)
        {
            return ("Invalid source or destination location.", null);
        }

        if (fromLocation.WarehouseId != toLocation.WarehouseId)
        {
            return ("Transfers must remain within the same warehouse.", null);
        }

        if (updatedTransfer.Items == null || !updatedTransfer.Items.Any())
        {
            return ("No items provided for the transfer.", null);
        }

        // Update basic fields
        existingTransfer.Reference = updatedTransfer.Reference;
        existingTransfer.TransferFrom = updatedTransfer.TransferFrom;
        existingTransfer.TransferTo = updatedTransfer.TransferTo;
        existingTransfer.UpdatedAt = DateTime.UtcNow;

        // Stock validation
        existingTransfer.Items.Clear();
        foreach (var item in updatedTransfer.Items)
        {
            var itemDetails = await _context.Items.FirstOrDefaultAsync(i => i.Uid == item.ItemId);
            if (itemDetails == null)
            {
                return ($"Item {item.ItemId} does not exist.", null);
            }

            // Validate stock in the source location
            if (!fromLocation.ItemAmounts.ContainsKey(item.ItemId) || fromLocation.ItemAmounts[item.ItemId] < item.Amount)
            {
                return ($"Not enough stock for item {item.ItemId} in the source location.", null);
            }

            // Validate destination location constraints
            if (itemDetails.Weight * item.Amount > toLocation.MaxWeight ||
                itemDetails.Width > toLocation.MaxWidth ||
                itemDetails.Height > toLocation.MaxHeight ||
                itemDetails.Depth > toLocation.MaxDepth)
            {
                return ($"Item {item.ItemId} exceeds destination location's constraints.", null);
            }

            existingTransfer.Items.Add(item);
        }

        await _context.SaveChangesAsync();

        return ("Transfer successfully updated.", existingTransfer);
    }



    public async Task<string> UpdateTransferStatusAsync(int transferId, string status)
    {
        var transfer = await _context.Transfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.TransferId == transferId);
        if (transfer == null)
        {
            return "Transfer not found.";
        }

        if ((transfer.TransferStatus == "Pending" && status == "InProgress") ||
            (transfer.TransferStatus == "InProgress" && status == "Completed"))
        {

            if (status == "InProgress")
            {
                var fromLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);
                if (fromLocation == null)
                {
                    return "Source location not found.";
                }

                foreach (var item in transfer.Items)
                {
                    if (!fromLocation.ItemAmounts.ContainsKey(item.ItemId))
                    {
                        return $"Item {item.ItemId} does not exist in the source location.";
                    }

                    if (fromLocation.ItemAmounts[item.ItemId] < item.Amount)
                    {
                        return $"Not enough stock for item {item.ItemId} in the source location.";
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
                    return "Destination location not found.";
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
        return "Invalid status transition or status. You can only update a transfer from 'Pending' to 'InProgress', or 'InProgress' to 'Completed'.";
    }

    public async Task<List<Transfer>> GetAllTransfersAsync()
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .ThenInclude(ti => ti.Item)
            .ToListAsync();
    }

    public async Task<List<Transfer>> GetAllTransfersAsync(int limit)
    {
        return await _context.Transfers
            .Include(t => t.Items)
            .ThenInclude(ti => ti.Item).Take(limit)
            .ToListAsync();
    }

    public async Task<List<Transfer>> GetAllTransfersPagedAsync(int limit, int page)
    {
        return await _context.Transfers.Skip(limit * (page - 1)).Take(limit).ToListAsync();
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
            return "Transfer not found.";
        }

        // Add back items to origin location if the transfer is incomplete
        if (transfer.TransferStatus != "Completed")
        {
            var fromLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == transfer.TransferFrom);

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

    public async Task<string> SoftDeleteTransferAsync(int transferId)
    {
        var transfer = await _context.Transfers.FindAsync(transferId);
        if (transfer == null)
        {
            return "Transfer not found.";
        }

        transfer.SoftDeleted = true;
        await _context.SaveChangesAsync();
        return "Transfer successfully soft deleted.";
    }
}

