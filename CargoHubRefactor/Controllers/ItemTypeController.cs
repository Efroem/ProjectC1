using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace CargoHubRefactor.Controllers{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Item_Types")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _itemTypeService;

        public ItemTypeController(IItemTypeService itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetItemTypes()
        {
            var item_types = _itemTypeService.GetItemTypesAsync();
            if (item_types == null)
            {
                return NotFound("No item lines found.");
            }

            return Ok(item_types);
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<ItemType>>> GetItemTypes(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show itemtypes with a limit below 1.");
            }

            var itemtypes = await _itemTypeService.GetItemTypesAsync(limit);
            if (itemtypes == null || !itemtypes.Any())
            {
                return NotFound("No itemtypes found.");
            }

            return Ok(itemtypes);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetItemTypesPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show ItemTypes with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var ItemTypes = await _itemTypeService.GetItemTypesPagedAsync(limit, page);
            if (ItemTypes == null || !ItemTypes.Any())
            {
                return NotFound("No ItemTypes found.");
            }

            return Ok(ItemTypes);
        }

        [HttpGet("{typeId}")]
        public async Task<ActionResult> GetItemTypeById(int typeId)
        {
            var item_type = await _itemTypeService.GetItemTypeByIdAsync(typeId);
            if (item_type == null)
            {
                return NotFound($"Item Type with ID: {typeId} not found.");
            }

            return Ok(item_type);
        }

        [HttpPost]
        public async Task<ActionResult> AddItemType([FromBody] ItemType itemLine)
        {
            var result = await _itemTypeService.AddItemTypeAsync(itemLine);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemType);
        }

        [HttpPut("{typeId}")]
        public async Task<ActionResult> UpdateItemType(int typeId, [FromBody] ItemType itemType)
        {
            var result = await _itemTypeService.UpdateItemTypeAsync(typeId, itemType);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemType);
        }

        [HttpDelete("{typeId}")]
        public async Task<ActionResult> DeleteItemType(int typeId)
        {
            var result = await _itemTypeService.DeleteItemTypeAsync(typeId);
            if (result == false)
            {
                return NotFound($"Item Type with ID: {typeId} not found.");
            }
            return Ok("Item Type succesfully deleted");
        }
    }
}