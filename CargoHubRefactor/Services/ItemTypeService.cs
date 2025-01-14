using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ItemTypeService : IItemTypeService
{
    private readonly CargoHubDbContext _context;

    public ItemTypeService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ItemType>> GetItemTypesAsync()
    {
        return await _context.ItemTypes.ToListAsync();
    }

    public async Task<IEnumerable<ItemType>> GetItemTypesAsync(int limit)
    {
        return await _context.ItemTypes.Take(limit).ToListAsync();
    }

    public async Task<ItemType?> GetItemTypeByIdAsync(int id)
    {
        return await _context.ItemTypes.FindAsync(id);
    }

    public async Task<(string message, ItemType? returnedItemType)> AddItemTypeAsync (ItemType itemType)
    {
        int nextId;

        if (string.IsNullOrWhiteSpace(itemType.Name))
            return ("Error: 'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemType.Description))
            return ("Error: 'Description' field must be filled in.", null);
        if (itemType.ItemLine <= 0)
            return ("Error: 'ItemType' must be a positive integer.", null);
        if (_context.ItemTypes.Any())
        {
            nextId = _context.ItemTypes.Max(c => c.TypeId) + 1;
        }
        else
        {
            nextId = 1;
        }


        var _itemType = new ItemType
        {
            TypeId = nextId,
            Name = itemType.Name,
            Description = itemType.Description,
            ItemLine = itemType.ItemLine,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        Console.WriteLine($"Attempting to insert LineId: {itemType.TypeId}");
        await _context.ItemTypes.AddAsync(_itemType);
        await _context.SaveChangesAsync();

        return ("", _itemType);
    }


    public async Task<(string message, ItemType? returnedItemType)> UpdateItemTypeAsync(int lineId, ItemType itemType)
    {
        var item_type = await _context.ItemTypes.FindAsync(lineId);
        if (item_type == null)
        {
            return ("Error: Item Line not found.", null);
        }

        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(itemType.Name))
            return ("Error: 'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemType.Description))
            return ("Error: 'Description' field must be filled in.", null);

   
        item_type.Name = itemType.Name;
        item_type.Description = itemType.Description;

        item_type.UpdatedAt = DateTime.Now; // Set UpdatedAt to current time

        await _context.SaveChangesAsync();
        return ("ItemType successfully updated.", item_type);
    }

    public async Task<bool> DeleteItemTypeAsync(int typeId)
    {
        var item_type = await _context.ItemTypes.FindAsync(typeId);
        if (item_type == null)
        {
            return false;
        }

        _context.ItemTypes.Remove(item_type);
        await _context.SaveChangesAsync();
        return true;
    }
}
