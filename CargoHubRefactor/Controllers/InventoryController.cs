using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CargoHubRefactor.Controllers{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Inventories")]
    [ApiController]

    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _InventoryService;

        public InventoryController(IInventoryService InventoryService)
        {
            _InventoryService = InventoryService;
        }

        [HttpGet]
        public async Task<ActionResult> GetInventories()
        {
            var inventories = _InventoryService.GetInventoriesAsync();
            if (inventories == null)
            {
                return NotFound("No item groups found.");
            }

            return Ok(inventories);
        }
                
        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show inventories with a limit below 1.");
            }

            var inventories = await _InventoryService.GetInventoriesAsync(limit);
            if (inventories == null || !inventories.Any())
            {
                return NotFound("No inventories found.");
            }

            return Ok(inventories);
        }
            
        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoriesPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show inventories with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var inventories = await _InventoryService.GetInventoriesPagedAsync(limit, page);
            if (inventories == null || !inventories.Any())
            {
                return NotFound("No inventories found.");
            }

            return Ok(inventories);
        }

        [HttpGet("{inventoryId}")]
        public async Task<ActionResult> GetInventoryById(int inventoryId)
        {
            var inventory = _InventoryService.GetInventoryByIdAsync(inventoryId);
            if (inventory.Result == null)
            {
                return NotFound($"Inventory with ID {inventoryId} not found.");
            }

            return Ok(inventory);
        }

        [HttpPost]
        public async Task<ActionResult> AddInventory([FromBody] Inventory Inventory)
        {
            var result = await _InventoryService.AddInventoryAsync(Inventory);
            if (result.returnedInventory == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedInventory);
        }

        [HttpPut("{inventoryId}")]
        public async Task<ActionResult> UpdateInventory(int inventoryId, [FromBody] Inventory Inventory)
        {
            var result = await _InventoryService.UpdateInventoryAsync(inventoryId, Inventory);
            if (result.returnedInventory == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedInventory);
        }

        [HttpDelete("{inventoryId}")]
        public async Task<ActionResult> DeleteInventory(int inventoryId)
        {
            var result = await _InventoryService.DeleteInventoryAsync(inventoryId);
            if (result == false)
            {
                return NotFound("Inventory not found");
            }
            return Ok("Successfully deleted inventory");
        }
        
        [HttpDelete("{inventoryId}/test")]
        public async Task<ActionResult> SoftDeleteInventory(int inventoryId)
        {
            var result = await _InventoryService.SoftDeleteInventoryAsync(inventoryId);
            if (result == false)
            {
                return NotFound("Error: Inventory not found");
            }
            return Ok("Successfully soft deleted inventory");
        }
    }
}