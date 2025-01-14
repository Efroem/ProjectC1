using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInventoryService
{
    Task<List<Inventory>> GetInventoriesAsync();
    Task<IEnumerable<Inventory>> GetInventoriesAsync(int limit);
    Task<Inventory?> GetInventoryByIdAsync(int id);
    Task<(string message, Inventory? returnedInventory)> AddInventoryAsync(Inventory Inventory);
    Task<(string message, Inventory? returnedInventory)> UpdateInventoryAsync(int id, Inventory Inventory);
    Task<bool> DeleteInventoryAsync(int id);
    Task<IEnumerable<Inventory>> GetInventoriesPagedAsync(int limit, int page);
}
