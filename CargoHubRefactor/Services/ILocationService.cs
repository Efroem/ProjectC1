using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILocationService
{
    Task<Location> GetLocationAsync(int locationId);
    Task<IEnumerable<Location>> GetLocationsAsync();
    Task<IEnumerable<Location>> GetLocationsAsync(int limit);
    Task<IEnumerable<Location>> GetLocationsPagedAsync(int limit, int page);
    Task<Location> AddLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(int id, Location location);
    Task<Location> UpdateLocationItemsAsync(int id, List<LocationItem> locationItems);
    Task<bool> DeleteLocationAsync(int locationId);
    Task<IEnumerable<Location>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<bool> IsValidLocationNameAsync(string name);
}
