using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [ApiController]
    [Route("api/v1/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transfer>>> GetAllTransfers()
        {
            return Ok(await _transferService.GetAllTransfersAsync());
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetAllTransfers(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Transfers with a limit below 1.");
            }

            var transfers = await _transferService.GetAllTransfersAsync(limit);
            if (transfers == null || !transfers.Any())
            {
                return NotFound("No Transfers found.");
            }

            return Ok(transfers);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetAllTransfersPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show Transfers with a limit below 1.");
            }

            var transfers = await _transferService.GetAllTransfersPagedAsync(limit, page);
            if (transfers == null || !transfers.Any())
            {
                return NotFound("No Transfers found.");
            }

            return Ok(transfers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transfer>> GetTransferById(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null) return NotFound("Transfer not found.");
            return Ok(transfer);
        }

        [HttpPost]
        public async Task<ActionResult> AddTransfer([FromBody] Transfer transfer)
        {
            var (message, createdTransfer) = await _transferService.AddTransferAsync(transfer);
            if (createdTransfer == null) return BadRequest(message);
            return CreatedAtAction(nameof(GetTransferById), new { id = createdTransfer.TransferId }, createdTransfer);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateTransferStatus(int id, [FromBody] string status)
        {
            var message = await _transferService.UpdateTransferStatusAsync(id, status);
            if (!message.Contains("Transfer status successfully updated.")) return BadRequest(message);
            return Ok(message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTransfer(int id, [FromBody] Transfer transfer)
        {
            var (message, updatedTransfer) = await _transferService.UpdateTransferAsync(id, transfer);
            if (updatedTransfer == null) return BadRequest(message);
            return Ok(updatedTransfer);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransfer(int id)
        {
            var message = await _transferService.DeleteTransferAsync(id);
            if (!message.Contains("Transfer successfully deleted.")) return NotFound(message);
            return Ok(message);
        }

        [HttpDelete("{id}/test")]
        public async Task<ActionResult> SoftDeleteTransfer(int id)
        {
            var message = await _transferService.SoftDeleteTransferAsync(id);
            if (!message.Contains("Transfer successfully soft deleted.")) return NotFound(message);
            return Ok(message);
        }
    }
}