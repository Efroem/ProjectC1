using System.Collections.Generic;
using System.Threading.Tasks;

public interface IItemService
{
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<IEnumerable<Item>> GetItemsAsync(int limit);
    Task<IEnumerable<Item>> GetItemsPagedAsync(int limit, int page);
    Task<Item?> GetItemByIdAsync(string uid);
    Task<int?> GetItemAmountAtLocationByIdAsync(string uid, int locationId);
    Task<(string message, Item? returnedItem)> AddItemAsync(Item item);
    Task<(string message, Item? returnedItem)> UpdateItemAsync(string id, Item item);
    Task<bool> DeleteItemAsync(string uid);
}
