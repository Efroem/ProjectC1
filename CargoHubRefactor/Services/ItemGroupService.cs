using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ItemGroupService : IItemGroupService
{
    private readonly CargoHubDbContext _context;

    public ItemGroupService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ItemGroup>> GetItemGroupsAsync()
    {
        return await _context.ItemGroups.ToListAsync();
    }

    public async Task<IEnumerable<ItemGroup>> GetItemGroupsAsync(int limit)
    {
        return await _context.ItemGroups.Take(limit).ToListAsync();
    }

    public async Task<ItemGroup?> GetItemGroupByIdAsync(int id)
    {
        return await _context.ItemGroups.FindAsync(id);
    }
    public async Task<IEnumerable<ItemGroup>> GetItemGroupsPagedAsync(int limit, int page)
    {
        return await _context.ItemGroups.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }

    public async Task<(string message, ItemGroup? returnedItemGroup)> AddItemGroupAsync (ItemGroup itemGroup)
    {
        int nextId;

        if (string.IsNullOrWhiteSpace(itemGroup.Name))
            return ("'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemGroup.Description))
            return ("'Description' field must be filled in.", null);

        if (_context.ItemGroups.Any())
        {
            nextId = _context.ItemGroups.Max(c => c.GroupId) + 1;
        }
        else
        {
            nextId = 1;
        }

        var _itemGroup = new ItemGroup
        {
            GroupId = nextId,
            Name = itemGroup.Name,
            Description = itemGroup.Description,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _context.ItemGroups.AddAsync(_itemGroup);
        await _context.SaveChangesAsync();

        return ("", _itemGroup);
    }


    public async Task<(string message, ItemGroup? returnedItemGroup)> UpdateItemGroupAsync(int groupId, ItemGroup itemGroup)
    {
        var item_group = await _context.ItemGroups.FindAsync(groupId);
        if (item_group == null)
        {
            return ("Item Group not found.", null);
        }

        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(itemGroup.Name))
            return ("'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(itemGroup.Description))
            return ("'Description' field must be filled in.", null);

   
        item_group.Name = itemGroup.Name;
        item_group.Description = itemGroup.Description;

        item_group.UpdatedAt = DateTime.Now; // Set UpdatedAt to current time

        await _context.SaveChangesAsync();
        return ("Item Group successfully updated.", item_group);
    }

    public async Task<bool> DeleteItemGroupAsync(int groupId)
    {
        var item_group = await _context.ItemGroups.FindAsync(groupId);
        if (item_group == null)
        {
            return false;
        }

        _context.ItemGroups.Remove(item_group);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteItemGroupAsync(int id)
    {
        var item_group = await _context.ItemGroups.FirstOrDefaultAsync(c => c.GroupId == id);
        if (item_group == null)
        {
            return false;
        }

        item_group.SoftDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
