using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<ActionResult> GetItems()
        {
            var item_s = await _itemService.GetItemsAsync();
            if (item_s == null)
            {
                return NotFound("No items found in the inventory.");
            }

            return Ok(item_s);
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show items with a limit below 1.");
            }

            var items = await _itemService.GetItemsAsync(limit);
            if (items == null || !items.Any())
            {
                return NotFound("No items found.");
            }

            return Ok(items);
        }
        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetItemsPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Items with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var Items = await _itemService.GetItemsPagedAsync(limit, page);
            if (Items == null || !Items.Any())
            {
                return NotFound("No Items found.");
            }

            return Ok(Items);
        }

        [HttpGet("{ItemId}")]
        public async Task<ActionResult> GetItemById(string ItemId)
        {
            var item_ = await _itemService.GetItemByIdAsync(ItemId);
            if (item_ == null)
            {
                return NotFound($"Item with ID: {ItemId} not found.");
            }

            return Ok(item_);
        }

        [HttpGet("{ItemId}/Locations/{LocationId}")]
        public async Task<ActionResult> GetItemAmountAtLocationById(string ItemId, int LocationId)
        {
            var itemAmount = await _itemService.GetItemAmountAtLocationByIdAsync(ItemId, LocationId);
            if (itemAmount == null)
            {
                return NotFound($"Item with ID '{ItemId}' not found at location ID {LocationId}.");
            }

            return Ok($"Location {LocationId} has {itemAmount} of Item {ItemId}");
        }

        [HttpPost]
        public async Task<ActionResult> AddItem([FromBody] Item item)
        {
            var result = await _itemService.AddItemAsync(item);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItem);
        }

        [HttpPut("{ItemId}")]
        public async Task<ActionResult> UpdateItem(string ItemId, [FromBody] Item item)
        {
            var result = await _itemService.UpdateItemAsync(ItemId, item);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItem);
        }

        [HttpDelete("{ItemId}")]
        public async Task<ActionResult> DeleteItem(string ItemId)
        {
            var result = await _itemService.DeleteItemAsync(ItemId);
            if (result == false)
            {
                return NotFound($"Item with ID: {ItemId} not found.");
            }
            return Ok("Item successfully deleted");
        }
    }
}