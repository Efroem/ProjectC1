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

        // Endpoint zonder limiet (alle warehouses ophalen)
        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            return Ok(await _warehouseService.GetAllWarehousesAsync());
        }

        // Endpoint met limiet (alle warehouses ophalen, maar beperkt tot het opgegeven aantal)
        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Warehouses with a limit below 1.");
            }

            var warehouses = await _warehouseService.GetAllWarehousesAsync(limit);
            if (warehouses == null || warehouses.Count == 0)
            {
                return NotFound("No warehouses found.");
            }

            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound("No warehouses found.");
            }
            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<ActionResult> AddWarehouse([FromBody] WarehouseDto warehouseDto)
        {
            var result = await _warehouseService.AddWarehouseAsync(warehouseDto);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.warehouse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDto warehouseDto)
        {
            (string message, Warehouse ReturnedWarehouse) result = await _warehouseService.UpdateWarehouseAsync(id, warehouseDto);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.ReturnedWarehouse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            if (result.StartsWith("Error"))
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
