using System.Collections.Generic;
using System.Threading.Tasks;

public interface IWarehouseService
{
    Task<List<Warehouse>> GetAllWarehousesAsync();
    Task<List<Warehouse>> GetAllWarehousesAsync(int limit);
    Task<Warehouse> GetWarehouseByIdAsync(int id);
    Task<(string message, Warehouse? warehouse)> AddWarehouseAsync(WarehouseDto warehouseDto);
    Task<(string message, Warehouse ReturnedWarehouse)> UpdateWarehouseAsync(int id, WarehouseDto warehouseDto);
    Task<string> DeleteWarehouseAsync(int id);
}