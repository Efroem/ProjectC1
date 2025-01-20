using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Models;

public class ItemService : IItemService
{
    private readonly CargoHubDbContext _context;

    public ItemService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await _context.Items.ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(int limit)
    {
        return await _context.Items.Take(limit).ToListAsync();
    }
    public async Task<IEnumerable<Item>> GetItemsPagedAsync(int limit, int page)
    {
        return await _context.Items.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }

    public async Task<Item?> GetItemByIdAsync(string uid)
    {
        return await _context.Items.FindAsync(uid);
    }

    public async Task<int?> GetItemAmountAtLocationByIdAsync(string uid, int locationId)
    {
        Location? location = await _context.Locations.FirstOrDefaultAsync(x => x.LocationId == locationId && x.ItemAmountsString.Contains(uid));
        if (location == null) return -1;
        return location.ItemAmounts[uid];
    }

    public async Task<(string message, Item? returnedItem)> AddItemAsync (Item item)
    {
        string nextId;


    if (string.IsNullOrWhiteSpace(item.Code))
        return ("'Name' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.Description))
        return ("'Description' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.ShortDescription))
        return ("'ShortDescription' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.UpcCode))
        return ("'UpcCode' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.ModelNumber))
        return ("'ModelNumber' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.CommodityCode))
        return ("'CommodityCode' field must be filled in.", null);
    if (item.ItemLine <= 0)
        return ("'ItemLine' must be a positive integer.", null);
    if (item.ItemGroup <= 0)
        return ("'ItemGroup' must be a positive integer.", null);
    if (item.ItemType <= 0)
        return ("'ItemType' must be a positive integer.", null);
    if (item.UnitPurchaseQuantity <= 0)
        return ("'UnitPurchaseQuantity' must be a positive integer.", null);
    if (item.UnitOrderQuantity <= 0)
        return ("'UnitOrderQuantity' must be a positive integer.", null);
    if (item.PackOrderQuantity <= 0)
        return ("'PackOrderQuantity' must be a positive integer.", null);
    if (item.SupplierId <= 0)
        return ("'SupplierId' must be a positive integer.", null);
    if (string.IsNullOrWhiteSpace(item.SupplierCode))
        return ("'SupplierCode' field must be filled in.", null);
    if (string.IsNullOrWhiteSpace(item.SupplierPartNumber))
        return ("'SupplierPartNumber' field must be filled in.", null);

    if (await _context.Items.AnyAsync(i => i.Code == item.Code))
        {
            return ("An Item with this Code already exists.", null);
        }
    if (await _context.Items.AnyAsync(i => i.UpcCode == item.UpcCode))
        {
            return ("An Item with this Upc Code already exists.", null);
        }
    if (await _context.Items.AnyAsync(i => i.ModelNumber == item.ModelNumber))
        {
            return ("An Item with this Model Number already exists.", null);
        }
    if (await _context.Items.AnyAsync(i => i.CommodityCode == item.CommodityCode))
        {
            return ("An Item with this Commodity Code already exists.", null);
        }

    // Check if supplier exists. if not. add supplier
    // REMOVE THE CODE PART WHERE IT ADDS THE SUPPLIER TO THE DATABASE 
    // AS SOON AS SUPPLIER ENDPOINT IS COMPLETE
    var supplier = await _context.Suppliers.FindAsync(item.SupplierId);
    if (supplier == null) {
        supplier = new Supplier{
            SupplierId = item.SupplierId,   
            Code = item.SupplierCode,
            Name = "",
            Address = "",
            AddressExtra = "",
            City = "",
            ZipCode = "",
            Province = "",
            Country = "",
            ContactName = "",
            PhoneNumber = "",
            Reference = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();
    }

    // Optionally validate related objects


        if (_context.Items.Any())
        {
            string? highestId = _context.Items.OrderByDescending(x => x.Uid)
                                    .Select(x => x.Uid)
                                    .FirstOrDefault();
            
            int highestIdInt = int.Parse(Regex.Match(highestId, @"\d+").Value);
            highestIdInt++;
            highestId = Convert.ToString(highestIdInt);
            nextId = $"P{String.Concat(Enumerable.Repeat("0", 6-highestId.Length))}{highestId}";
        }
        else
        {
            nextId = "P000001";
        }

        var _item = new Item
        {
            Uid = nextId,
            Code = item.Code,
            Description = item.Description,
            ShortDescription = item.ShortDescription,
            UpcCode = item.UpcCode,
            ModelNumber = item.ModelNumber,
            CommodityCode = item.CommodityCode,
            ItemLine = item.ItemLine,
            ItemGroup = item.ItemGroup,
            ItemType = item.ItemType,
            Height = item.Height,
            Width = item.Width,
            Depth = item.Depth,
            Weight = item.Weight,
            Price = item.Price,
            Classification = item.Classification,
            UnitPurchaseQuantity = item.UnitPurchaseQuantity,
            UnitOrderQuantity = item.UnitOrderQuantity,
            PackOrderQuantity = item.PackOrderQuantity,
            SupplierId = item.SupplierId,
            SupplierCode = item.SupplierCode,
            SupplierPartNumber = item.SupplierPartNumber,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        Console.Write($"Attempting to insert Id: {item.Uid}");
        await _context.Items.AddAsync(_item);
        await _context.SaveChangesAsync();

        return ("", _item);
    }


    public async Task<(string message, Item? returnedItem)> UpdateItemAsync(string Id, Item item)
    {
        var item_ = await _context.Items.FindAsync(Id);
        if (item_ == null)
        {
            return ("Item  not found.", null);
        }

        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(item.Code))
            return ("'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.Description))
            return ("'Description' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.ShortDescription))
            return ("'ShortDescription' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.UpcCode))
            return ("'UpcCode' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.ModelNumber))
            return ("'ModelNumber' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.CommodityCode))
            return ("'CommodityCode' field must be filled in.", null);
        if (item.ItemLine <= 0)
            return ("'ItemLine' must be a positive integer.", null);
        if (item.ItemGroup <= 0)
            return ("'ItemGroup' must be a positive integer.", null);
        if (item.ItemType <= 0)
            return ("'ItemType' must be a positive integer.", null);
        if (item.UnitPurchaseQuantity <= 0)
            return ("'UnitPurchaseQuantity' must be a positive integer.", null);
        if (item.UnitOrderQuantity <= 0)
            return ("'UnitOrderQuantity' must be a positive integer.", null);
        if (item.PackOrderQuantity <= 0)
            return ("'PackOrderQuantity' must be a positive integer.", null);
        if (item.SupplierId <= 0)
            return ("'SupplierId' must be a positive integer.", null);
        if (string.IsNullOrWhiteSpace(item.SupplierCode))
            return ("'SupplierCode' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(item.SupplierPartNumber))
            return ("'SupplierPartNumber' field must be filled in.", null);


        if (await _context.Items.AnyAsync(i => i.Code == item_.Code && item.Code == item_.Code))
            {
                return ("An Item with this Code already exists.", null);
            }
        if (await _context.Items.AnyAsync(i => i.UpcCode == item_.UpcCode && item.UpcCode == item_.UpcCode))
            {
                return ("An Item with this Upc Code already exists.", null);
            }
        if (await _context.Items.AnyAsync(i => i.ModelNumber == item_.ModelNumber && item.ModelNumber == item_.ModelNumber))
            {
                return ("An Item with this Model Number already exists.", null);
            }
        if (await _context.Items.AnyAsync(i => i.CommodityCode == item_.CommodityCode && item.CommodityCode == item_.CommodityCode))
            {
                return ("An Item with this Commodity Code already exists.", null);
            }
   

        item_.Code = item.Code;
        item_.Description = item.Description;
        item_.ShortDescription = item.ShortDescription;
        item_.UpcCode = item.UpcCode;
        item_.ModelNumber = item.ModelNumber;
        item_.CommodityCode = item.CommodityCode;
        item_.ItemLine = item.ItemLine;
        item_.ItemGroup = item.ItemGroup;
        item_.ItemType = item.ItemType;
        item_.Height = item.Height;
        item_.Width = item.Width;
        item_.Depth = item.Depth;
        item_.Weight = item.Weight;
        item_.Price = item.Price;
        item_.Classification = item.Classification;
        item_.UnitPurchaseQuantity = item.UnitPurchaseQuantity;
        item_.UnitOrderQuantity = item.UnitOrderQuantity;
        item_.PackOrderQuantity = item.PackOrderQuantity;
        item_.SupplierId = item.SupplierId;
        item_.SupplierCode = item.SupplierCode;
        item_.SupplierPartNumber = item.SupplierPartNumber;
        item_.UpdatedAt = DateTime.Now; // Set UpdatedAt to current time

        await _context.SaveChangesAsync();
        return ("Item successfully updated.", item_);
    }

    public async Task<bool> DeleteItemAsync(string Uid)
    {
        var item_ = await _context.Items.FindAsync(Uid);
        if (item_ == null)
        {
            return false;
        }

        _context.Items.Remove(item_);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteItemAsync(string uid)
    {
        var item = await _context.Items.FirstOrDefaultAsync(c => c.Uid == uid);
        if (item == null)
        {
            return false;
        }

        item.SoftDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
