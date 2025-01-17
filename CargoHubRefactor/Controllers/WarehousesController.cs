using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CargoHubRefactor.Controllers
{   
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/warehouses")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            return Ok(await _warehouseService.GetAllWarehousesAsync());
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllWarehouses(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Warehouses with a limit below 1.");
            }

            var warehouses = await _warehouseService.GetAllWarehousesAsync(limit);
            if (warehouses == null || !warehouses.Any())
            {
                return NotFound("No Warehouses found.");
            }

            return Ok(warehouses);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllTWarehousesPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Warehouses with a limit below 1.");
            }

            var warehouses = await _warehouseService.GetAllWarehousesPagedAsync(limit, page);
            if (warehouses == null || !warehouses.Any())
            {
                return NotFound("No Warehouses found.");
            }

            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound("No Warehouses found.");
            }
            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<ActionResult> AddWarehouse([FromBody] WarehouseDto warehouseDto)
        {
            var result = await _warehouseService.AddWarehouseAsync(warehouseDto);
            if (result.warehouse == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.warehouse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDto warehouseDto)
        {
            (string message, Warehouse ReturnedWarehouse) result = await _warehouseService.UpdateWarehouseAsync(id, warehouseDto);
            if (result.ReturnedWarehouse == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.ReturnedWarehouse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            if (!result.Contains("Warehouse successfully deleted."))
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}/test")]
        public async Task<ActionResult> SoftDeleteWarehouse(int id)
        {
            var result = await _warehouseService.SoftDeleteWarehouseAsync(id);
            if (!result.Contains("Warehouse successfully soft deleted."))
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}