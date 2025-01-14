using System.Collections.Generic;
using System.Threading.Tasks;

public interface IItemTypeService
{
    Task<IEnumerable<ItemType>> GetItemTypesAsync();
    Task<IEnumerable<ItemType>> GetItemTypesAsync(int limit);
    Task<ItemType?> GetItemTypeByIdAsync(int id);
    Task<(string message, ItemType? returnedItemType)> AddItemTypeAsync(ItemType ItemType);
    Task<(string message, ItemType? returnedItemType)> UpdateItemTypeAsync(int id, ItemType ItemType);
    Task<bool> DeleteItemTypeAsync(int id);
}
