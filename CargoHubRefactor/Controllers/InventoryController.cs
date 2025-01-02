using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CargoHubRefactor.Controllers{
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
            var inventories = await _InventoryService.GetInventoriesAsync();
            if (inventories == null)
            {
                return NotFound("No item groups found.");
            }

            return Ok(inventories);
        }

        [HttpGet("{inventoryId}")]
        public async Task<ActionResult> GetInventoryById(int inventoryId)
        {
            var inventory = await _InventoryService.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
            {
                return NotFound($"Item Group with ID {inventoryId} not found.");
            }

            return Ok(inventory);
        }

        [HttpPost]
        public async Task<ActionResult> AddInventory([FromBody] Inventory Inventory)
        {
            var result = await _InventoryService.AddInventoryAsync(Inventory);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedInventory);
        }

        [HttpPut("{inventoryId}")]
        public async Task<ActionResult> UpdateInventory(int inventoryId, [FromBody] Inventory Inventory)
        {
            var result = await _InventoryService.UpdateInventoryAsync(inventoryId, Inventory);
            if (result.message.StartsWith("Error"))
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
                return NotFound("Error: Inventory not found");
            }
            return Ok("Successfully deleted inventory");
        }
    }
}