using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ItemLineService : IItemLineService
{
    private readonly CargoHubDbContext _context;

    public ItemLineService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ItemLine>> GetItemLinesAsync()
    {
        return await _context.ItemLines.ToListAsync();
    }

    public async Task<IEnumerable<ItemLine>> GetItemLinesAsync(int limit)
    {
        return await _context.ItemLines.Take(limit).ToListAsync();
    }
    public async Task<IEnumerable<ItemLine>> GetItemLinesPagedAsync(int limit, int page)
    {
        return await _context.ItemLines.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }

    public async Task<ItemLine?> GetItemLineByIdAsync(int id)
    {
        return await _context.ItemLines.FindAsync(id);
    }

    public async Task<(string message, ItemLine? returnedItemLine)> AddItemLineAsync (ItemLine itemLine)
    {
        int nextId;

        if (string.IsNullOrWhiteSpace(itemLine.Name))
            return ("'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemLine.Description))
            return ("'Description' field must be filled in.", null);
        if (itemLine.ItemGroup <= 0)
            return ("'ItemGroup' must be a positive integer.", null);

        if (_context.ItemLines.Any())
        {
            nextId = _context.ItemLines.Max(c => c.LineId) + 1;
        }
        else
        {
            nextId = 1;
        }

        var _itemLine = new ItemLine
        {
            LineId = nextId,
            Name = itemLine.Name,
            Description = itemLine.Description,
            ItemGroup = itemLine.ItemGroup,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        Console.WriteLine($"Attempting to insert LineId: {itemLine.LineId}");
        await _context.ItemLines.AddAsync(_itemLine);
        await _context.SaveChangesAsync();

        return ("", _itemLine);
    }


    public async Task<(string message, ItemLine? returnedItemLine)> UpdateItemLineAsync(int lineId, ItemLine itemLine)
    {
        var item_line = await _context.ItemLines.FindAsync(lineId);
        if (item_line == null)
        {
            return ("Item Line not found.", null);
        }

        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(itemLine.Name))
            return ("'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemLine.Description))
            return ("'Description' field must be filled in.", null);

   
        item_line.Name = itemLine.Name;
        item_line.Description = itemLine.Description;

        item_line.UpdatedAt = DateTime.Now; // Set UpdatedAt to current time

        await _context.SaveChangesAsync();
        return ("Item Line successfully updated.", item_line);
    }

    public async Task<bool> DeleteItemLineAsync(int lineId)
    {
        var item_line = await _context.ItemLines.FindAsync(lineId);
        if (item_line == null)
        {
            return false;
        }

        _context.ItemLines.Remove(item_line);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> SoftDeleteItemLineAsync(int id)
    {
        var itemLine = await _context.ItemLines.FirstOrDefaultAsync(c => c.LineId == id);
        if (itemLine == null)
        {
            return false;
        }

        itemLine.SoftDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
