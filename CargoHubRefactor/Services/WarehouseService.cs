using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class WarehouseService : IWarehouseService
{
    private readonly CargoHubDbContext _context;

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
        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(warehouseDto.Code))
            return ("Error: 'Code' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Name))
            return ("Error: 'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Address))
            return ("Error: 'Address' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Zip))
            return ("Error: 'Zip' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.City))
            return ("Error: 'City' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Province))
            return ("Error: 'Province' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Country))
            return ("Error: 'Country' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactName))
            return ("Error: 'ContactName' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactPhone))
            return ("Error: 'ContactPhone' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactEmail))
            return ("Error: 'ContactEmail' field must be filled in.", null);

        // Check for duplicate code
        if (await _context.Warehouses.AnyAsync(w => w.Code == warehouseDto.Code))
        {
            return ("Error: A warehouse with this code already exists.", null);
        }

        int nextId;

        if (_context.Warehouses.Any())
        {
            nextId = _context.Warehouses.Max(w => w.WarehouseId) + 1;
        }
        else
        {
            nextId = 1;
        }

        var warehouse = new Warehouse
        {
            WarehouseId = nextId,
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
            CreatedAt = DateTime.Now, // Set CreatedAt to current time
            UpdatedAt = DateTime.Now  // Initialize UpdatedAt as well
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
        return ("Warehouse successfully created.", warehouse);
    }

    public async Task<(string message, Warehouse? ReturnedWarehouse)> UpdateWarehouseAsync(int id, WarehouseDto warehouseDto)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return ("Error: Warehouse not found.", null);
        }

        // Validate that all fields are filled in
        if (string.IsNullOrWhiteSpace(warehouseDto.Code))
            return ("Error: 'Code' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Name))
            return ("Error: 'Name' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Address))
            return ("Error: 'Address' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Zip))
            return ("Error: 'Zip' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.City))
            return ("Error: 'City' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Province))
            return ("Error: 'Province' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.Country))
            return ("Error: 'Country' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactName))
            return ("Error: 'ContactName' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactPhone))
            return ("Error: 'ContactPhone' field must be filled in.", null);
        if (string.IsNullOrWhiteSpace(warehouseDto.ContactEmail))
            return ("Error: 'ContactEmail' field must be filled in.", null);

        // Check for duplicate code, excluding the current warehouse
        if (await _context.Warehouses.AnyAsync(w => w.Code == warehouseDto.Code && w.WarehouseId != id))
        {
            return ("Error: A warehouse with this code already exists.", null);
        }

        warehouse.Code = warehouseDto.Code;
        warehouse.Name = warehouseDto.Name;
        warehouse.Address = warehouseDto.Address;
        warehouse.Zip = warehouseDto.Zip;
        warehouse.City = warehouseDto.City;
        warehouse.Province = warehouseDto.Province;
        warehouse.Country = warehouseDto.Country;
        warehouse.ContactName = warehouseDto.ContactName;
        warehouse.ContactPhone = warehouseDto.ContactPhone;
        warehouse.ContactEmail = warehouseDto.ContactEmail;
        warehouse.UpdatedAt = DateTime.Now; // Set UpdatedAt to current time

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
