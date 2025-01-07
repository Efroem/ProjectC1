using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace CargoHubRefactor.Controllers{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Item_Groups")]
    [ApiController]
    public class ItemGroupController : ControllerBase
    {
        private readonly IItemGroupService _itemGroupService;

        public ItemGroupController(IItemGroupService itemGroupService)
        {
            _itemGroupService = itemGroupService;
        }

        [HttpGet]
        public async Task<ActionResult> GetItemGroups()
        {
            var item_groups = _itemGroupService.GetItemGroupsAsync();
            if (item_groups == null)
            {
                return NotFound("No item groups found.");
            }

            return Ok(item_groups);
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<ItemGroup>>> GetItemGroups(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show clients with a limit below 1.");
            }

            var itemgroups = await _itemGroupService.GetItemGroupsAsync(limit);
            if (itemgroups == null || !itemgroups.Any())
            {
                return NotFound("No clients found.");
            }

            return Ok(itemgroups);
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult> GetItemGroupById(int groupId)
        {
            var item_group = _itemGroupService.GetItemGroupByIdAsync(groupId);
            if (item_group.Result == null)
            {
                return NotFound($"Item Group with ID: {groupId} not found.");
            }

            return Ok(item_group);
        }

        [HttpPost]
        public async Task<ActionResult> AddItemGroup([FromBody] ItemGroup itemGroup)
        {
            var result = await _itemGroupService.AddItemGroupAsync(itemGroup);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemGroup);
        }

        [HttpPut("{groupId}")]
        public async Task<ActionResult> UpdateItemGroup(int groupId, [FromBody] ItemGroup itemGroup)
        {
            var result = await _itemGroupService.UpdateItemGroupAsync(groupId, itemGroup);
            if (result.message.StartsWith("Error"))
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemGroup);
        }

        [HttpDelete("{groupId}")]
        public async Task<ActionResult> DeleteItemGroup(int groupId)
        {
            var result = await _itemGroupService.DeleteItemGroupAsync(groupId);
            if (result == false)
            {
                return NotFound($"Item Group with ID: {groupId} not found.");
            }
            return Ok("Successfully deleted Item Group");
        }
    }
}