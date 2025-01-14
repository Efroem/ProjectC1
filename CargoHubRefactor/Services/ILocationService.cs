using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILocationService
{
    Task<Location> GetLocationAsync(int locationId);
    Task<IEnumerable<Location>> GetLocationsAsync();
    Task<IEnumerable<Location>> GetLocationsAsync(int limit);
    Task<Location> AddLocationAsync(Location Location);
    Task<Location> UpdateLocationAsync(int id, Location Location);
    Task<Location> UpdateLocationItemsAsync(int id, List<LocationItem> LocationItems);
    Task<bool> DeleteLocationAsync(int locationId);
    Task<bool> SoftDeleteLocationAsync(int locationId);
    Task<IEnumerable<Location>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<bool> IsValidLocationNameAsync(string name);}
