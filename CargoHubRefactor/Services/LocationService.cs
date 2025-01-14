using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class LocationService : ILocationService
{
    private readonly CargoHubDbContext _context;

    public LocationService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<Location> GetLocationAsync(int locationId)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
    }

    public async Task<IEnumerable<Location>> GetLocationsAsync()
    {
        return await _context.Locations.ToListAsync();
    }

    public async Task<IEnumerable<Location>> GetLocationsAsync(int limit)
    {
        return await _context.Locations.Take(limit).ToListAsync();
    }
    public async Task<IEnumerable<Location>> GetLocationsPagedAsync(int limit, int page)
    {
        return await _context.Locations.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }
    public async Task<Location> AddLocationAsync(Location Location)
    {
        int nextId;

        if (!_context.Warehouses.Any(x => x.WarehouseId == Location.WarehouseId)) {
            return null;
        }
        
        if (_context.Locations.Any())
        {
            nextId = _context.Locations.Max(l => l.LocationId) + 1;
        }

        else
        {
            nextId = 1;
        }

        var location = new Location
        {
            LocationId = nextId,
            Name = Location.Name,
            Code = Location.Code,
            WarehouseId = Location.WarehouseId,
            ItemAmounts = new Dictionary<string, int>(),
            MaxDepth = Location.MaxDepth,
            MaxWeight = Location.MaxWeight,
            MaxHeight = Location.MaxHeight,
            MaxWidth = Location.MaxWidth,
            IsDock = Location.IsDock,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return location;
    }

    public async Task<Location> UpdateLocationAsync(int id, Location Location)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);
        if (location == null)
        {
            return null;
        }


        // Update Name if Location.Name is not null or empty
        if (!string.IsNullOrEmpty(Location.Name))
        {
            location.Name = Location.Name;
        }

        // Update Code if Location.Code is not null or empty
        if (!string.IsNullOrEmpty(Location.Code))
        {
            location.Code = Location.Code;
        }

        // Update WarehouseId if Location.WarehouseId is not null (assuming it's nullable)
        if (Location.WarehouseId != 0)
        {
            location.WarehouseId = Location.WarehouseId;
        }

        // // Initialize or update ItemAmounts dictionary if Location.ItemAmounts is not null
        // if (Location.ItemAmounts != null && Location.ItemAmounts.Any())
        // {
        //     location.ItemAmounts = Location.ItemAmounts;
        // }

        // Update MaxDepth if Location.MaxDepth is not null
        if (Location.MaxDepth != 0)
        {
            location.MaxDepth = Location.MaxDepth;
        }

        // Update MaxWeight if Location.MaxWeight is not null
        if (Location.MaxWeight != 0)
        {
            location.MaxWeight = Location.MaxWeight;
        }

        // Update MaxHeight if Location.MaxHeight is not null
        if (Location.MaxHeight != 0)
        {
            location.MaxHeight = Location.MaxHeight;
        }

        // Update MaxWidth if Location.MaxWidth is not null
        if (Location.MaxWidth != 0)
        {
            location.MaxWidth = Location.MaxWidth;
        }

        // Update IsDock if Location.IsDock has a valid value
        location.IsDock = Location.IsDock; // Assuming you want to directly update this without null check.

        // Update CreatedAt with the current UTC time
        location.CreatedAt = DateTime.UtcNow;

        // Update UpdatedAt with the current UTC time
        location.UpdatedAt = DateTime.UtcNow;
    
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();

        return location;
    }

    public async Task<Location> UpdateLocationItemsAsync(int id, List<LocationItem> LocationItems)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);
        if (location == null)
        {
            return null;
        }
        foreach (LocationItem ItemToAdd in LocationItems) {
            var warehouse = await _context.Warehouses.FindAsync(location.WarehouseId);
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ItemId == ItemToAdd.ItemId);
            if (inventory == null) continue;
            if (warehouse == null) continue;
            List<string> RestrictedClassifications = warehouse.RestrictedClassificationsList != null ? warehouse.RestrictedClassificationsList : new List<string>();



            if (location.MaxHeight != 0 && ItemToAdd.Height > location.MaxHeight ||
                location.MaxWidth != 0 && ItemToAdd.Width > location.MaxWidth ||
                location.MaxDepth != 0 && ItemToAdd.Depth > location.MaxDepth ||
                location.MaxWeight != 0 && ItemToAdd.Weight > location.MaxWeight ||
                RestrictedClassifications.Contains(ItemToAdd.Classification) 
            ) continue;

            if (inventory.LocationsList == null)
            {
                inventory.LocationsList = new List<int>();  // Initialize LocationsList if null
            }
            inventory.LocationsList.Add(location.LocationId);


            if (location.ItemAmounts == null) {
                location.ItemAmounts = new Dictionary<string, int>();
            }
            
            if (location.ItemAmounts.ContainsKey(ItemToAdd.ItemId)) {
                location.ItemAmounts[ItemToAdd.ItemId] += ItemToAdd.Amount;
            }
            else {
                location.ItemAmounts.Add(ItemToAdd.ItemId, ItemToAdd.Amount);
            }
            inventory.TotalOnHand += ItemToAdd.Amount;
        }
        location.UpdatedAt = DateTime.UtcNow;
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<bool> DeleteLocationAsync(int locationId)
    {
        var location = await _context.Locations.FindAsync(locationId);
        if (location == null)
        {
             return false;
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Location>> GetLocationsByWarehouseAsync(int warehouseId)
    {
        return await _context.Locations
                             .Where(l => l.WarehouseId == warehouseId)
                             .Include(l => l.Warehouse)
                             .ToListAsync();
    }
     public async Task<bool> IsValidLocationNameAsync(string name)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"^Row: ([A-Z]), Rack: (\d+), Shelf: (\d+)$");
        var match = regex.Match(name);

        if (!match.Success)
        {
            return false;
        }

        var row = match.Groups[1].Value;
        var rack = int.Parse(match.Groups[2].Value);
        var shelf = int.Parse(match.Groups[3].Value);

        if (row.Length != 1 || row[0] < 'A' || row[0] > 'Z')
        {
            return false; 
        }

        if (rack < 1 || rack > 100)
        {
            return false; 
        }

        if (shelf < 0 || shelf > 10)
        {
            return false; 
        }

        return true;
    }

}
