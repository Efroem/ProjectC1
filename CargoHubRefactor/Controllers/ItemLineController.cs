using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace CargoHubRefactor.Controllers{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Item_Lines")]
    [ApiController]
    public class ItemLineController : ControllerBase
    {
        private readonly IItemLineService _itemLineService;

        public ItemLineController(IItemLineService itemLineService)
        {
            _itemLineService = itemLineService;
        }

         [HttpGet("{lineId}")]
        public async Task<ActionResult> GetItemLineById(int lineId)
        {
            var item_line = await _itemLineService.GetItemLineByIdAsync(lineId);
            if (item_line == null)
            {
                return NotFound($"Item Line with ID: {lineId} not found.");
            }

            return Ok(item_line);
        }

        [HttpGet]
        public async Task<ActionResult> GetItemLines()
        {
            var item_lines = await _itemLineService.GetItemLinesAsync();
            if (item_lines == null || !item_lines.Any())
            {
                return NotFound("No Item Lines found");
            }

            return Ok(item_lines);
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<ItemLine>>> GetItemLines(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Item Lines with a limit below 1.");
            }

            var itemlines = await _itemLineService.GetItemLinesAsync(limit);
            if (itemlines == null || !itemlines.Any())
            {
                return NotFound("No Item Lines found.");
            }

            return Ok(itemlines);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<ItemLine>>> GetItemLinesPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Item Lines with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var ItemLines = await _itemLineService.GetItemLinesPagedAsync(limit, page);
            if (ItemLines == null || !ItemLines.Any())
            {
                return NotFound("No Item Lines found.");
            }

            return Ok(ItemLines);
        }


        [HttpPost]
        public async Task<ActionResult> AddItemLine([FromBody] ItemLine itemLine)
        {
            var result = await _itemLineService.AddItemLineAsync(itemLine);
            if (result.returnedItemLine == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemLine);
        }

        [HttpPut("{lineId}")]
        public async Task<ActionResult> UpdateItemLine(int lineId, [FromBody] ItemLine itemLine)
        {
            var result = await _itemLineService.UpdateItemLineAsync(lineId, itemLine);
            if (result.returnedItemLine == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result.returnedItemLine);
        }

        [HttpDelete("{lineId}")]
        public async Task<ActionResult> DeleteItemLine(int lineId)
        {
            var result = await _itemLineService.DeleteItemLineAsync(lineId);
            if (result == false)
            {
                return NotFound($"Item Line with ID: {lineId} not found.");
            }
            return Ok("Successfully deleted Item Line");
        }

        [HttpDelete("{lineId}/test")]
        public async Task<ActionResult> SoftDeleteItemLine(int lineId)
        {
            var result = await _itemLineService.SoftDeleteItemLineAsync(lineId);
            if (result == false)
            {
                return NotFound($"Item Line with ID: {lineId} not found.");
            }
            return Ok("Successfully soft deleted Item Line");
        }
    }
}