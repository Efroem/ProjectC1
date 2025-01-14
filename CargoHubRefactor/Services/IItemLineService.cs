using System.Collections.Generic;
using System.Threading.Tasks;

public interface IItemLineService
{
    Task<IEnumerable<ItemLine>> GetItemLinesAsync();
    Task<IEnumerable<ItemLine>> GetItemLinesAsync(int limit);
    Task<IEnumerable<ItemLine>> GetItemLinesPagedAsync(int limit, int page);
    Task<ItemLine?> GetItemLineByIdAsync(int id);
    Task<(string message, ItemLine? returnedItemLine)> AddItemLineAsync(ItemLine ItemLine);
    Task<(string message, ItemLine? returnedItemLine)> UpdateItemLineAsync(int id, ItemLine ItemLine);
    Task<bool> DeleteItemLineAsync(int id);
}
