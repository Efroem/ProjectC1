using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = await _locationService.GetLocationAsync(id);
            if (location == null)
            {
                return NotFound($"Location with ID: {id} was not found");
            }
            return Ok(location);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _locationService.GetLocationsAsync();
            if (locations == null || !locations.Any())
            {
                return NotFound($"No locations were found");
            }
            return Ok(locations);
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetLocationsByWarehouse(int warehouseId)
        {
            var locations = await _locationService.GetLocationsByWarehouseAsync(warehouseId);
            return Ok(locations);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            if (location == null || string.IsNullOrEmpty(location.Name) || string.IsNullOrEmpty(location.Code))
            {
                return BadRequest("Location name and code are required.");
            }
            if (location.WarehouseId < 0)
            {
                return BadRequest("Enter a valid WarehouseID");
            }

            if (!await _locationService.IsValidLocationNameAsync(location.Name))
            {
                return BadRequest("Location name must follow the format: 'Row: A, Rack: 1, Shelf: 0'. Row must be between A-Z, Rack between 1-100, and Shelf between 0-10.");
            }

            var createdLocation = await _locationService.AddLocationAsync(location);
            if (createdLocation != null)
            {
                return Ok(createdLocation);
            }

            return BadRequest("Failed to add Location. Invalid data found");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location location)
        {
            if (id != location.LocationId || string.IsNullOrEmpty(location.Name) || string.IsNullOrEmpty(location.Code))
            {
                return BadRequest("Please provide values for all required fields.");
            }

            if (!await _locationService.IsValidLocationNameAsync(location.Name))
            {
                return BadRequest("Location name must follow the format: 'Row: A, Rack: 1, Shelf: 0'. Row must be between A-Z, Rack between 1-100, and Shelf between 0-10.");
            }

            var updatedLocation = await _locationService.UpdateLocationAsync(id, location);
            if (updatedLocation == null)
            {
                return NotFound($"Location with ID: {id} was not found");
            }

            return Ok(updatedLocation);
        }

        [HttpPut("{id}/Items")]
        public async Task<IActionResult> UpdateLocationItems(int id, [FromBody] List<LocationItem> LocationItems)
        {
            foreach (LocationItem item in LocationItems)
            {
                if (item.ItemId.IsNullOrEmpty())
                {
                    return BadRequest("Invalid ItemId in list ofItems to add");
                }
            }

            var updatedLocation = await _locationService.UpdateLocationItemsAsync(id, LocationItems);
            if (updatedLocation == null)
            {
                return NotFound($"Location with ID: {id} was not found");
            }

            return Ok(updatedLocation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var result = await _locationService.DeleteLocationAsync(id);
            if (!result)
            {
                return NotFound($"Location with ID: {id} not found");
            }
            return Ok($"Location with ID: {id} successfully deleted");
        }
    }
}