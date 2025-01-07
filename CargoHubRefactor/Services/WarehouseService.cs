using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class WarehouseService : IWarehouseService
{
    private readonly CargoHubDbContext _context;

    private static readonly HashSet<string> DangerousGoodsClassifications = new HashSet<string>
    {
        "1.1", "1.2", "1.3", "1.4", "1.5", "1.6",
        "2.1", "2.2", "2.3",
        "3",
        "4.1", "4.2", "4.3",
        "5.1", "5.2",
        "6.1", "6.2",
        "7",
        "8",
        "9"
    };

    public WarehouseService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<Warehouse>> GetAllWarehousesAsync()
    {
        return await _context.Warehouses.ToListAsync();
    }

    public async Task<Warehouse> GetWarehouseByIdAsync(int id)
    {
        return await _context.Warehouses.FindAsync(id);
    }

    public async Task<(string message, Warehouse? warehouse)> AddWarehouseAsync(WarehouseDto warehouseDto)
    {
        if (string.IsNullOrWhiteSpace(warehouseDto.Code))
            return ("Error: 'Code' field must be filled in.", null);
        if (warehouseDto.RestrictedClassificationsList != null)
        {
            foreach (var classification in warehouseDto.RestrictedClassificationsList)
            {
                if (!DangerousGoodsClassifications.Contains(classification))
                {
                    return ($"Error: Invalid classification '{classification}'.", null);
                }
            }
        }

        var warehouse = new Warehouse
        {
            Code = warehouseDto.Code,
            Name = warehouseDto.Name,
            Address = warehouseDto.Address,
            Zip = warehouseDto.Zip,
            City = warehouseDto.City,
            Province = warehouseDto.Province,
            Country = warehouseDto.Country,
            ContactName = warehouseDto.ContactName,
            ContactPhone = warehouseDto.ContactPhone,
            ContactEmail = warehouseDto.ContactEmail,
            RestrictedClassificationsList = warehouseDto.RestrictedClassificationsList,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
        return ("Warehouse successfully created.", warehouse);
    }

    public async Task<(string message, Warehouse ReturnedWarehouse)> UpdateWarehouseAsync(int id, WarehouseDto warehouseDto)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return ("Error: Warehouse not found.", null);
        }
        if (warehouseDto.RestrictedClassificationsList != null)
        {
            foreach (var classification in warehouseDto.RestrictedClassificationsList)
            {
                if (!DangerousGoodsClassifications.Contains(classification))
                {
                    return ($"Error: Invalid classification '{classification}'.", null);
                }
            }
        }

        warehouse.RestrictedClassificationsList = warehouseDto.RestrictedClassificationsList;
        warehouse.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return ("Warehouse successfully updated.", warehouse);
    }

    public async Task<string> DeleteWarehouseAsync(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return "Error: Warehouse not found.";
        }

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();
        return "Warehouse successfully deleted.";
    }
}