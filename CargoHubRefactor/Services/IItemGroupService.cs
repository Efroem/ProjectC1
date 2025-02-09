using System.Collections.Generic;
using System.Threading.Tasks;

public interface IItemGroupService
{
    Task<IEnumerable<ItemGroup>> GetItemGroupsAsync();
    Task<IEnumerable<ItemGroup>> GetItemGroupsAsync(int limit);
    Task<IEnumerable<ItemGroup>> GetItemGroupsPagedAsync(int limit, int page);
    Task<ItemGroup?> GetItemGroupByIdAsync(int id);
    Task<(string message, ItemGroup? returnedItemGroup)> AddItemGroupAsync(ItemGroup itemGroup);
    Task<(string message, ItemGroup? returnedItemGroup)> UpdateItemGroupAsync(int id, ItemGroup itemGroup);
    Task<bool> DeleteItemGroupAsync(int id);
    Task<bool> SoftDeleteItemGroupAsync(int id);
}
