using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoHubRefactor.Controllers
{

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
            if (message.StartsWith("Error")) return BadRequest(message);
            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransfer(int id)
        {
            var message = await _transferService.DeleteTransferAsync(id);
            if (message.StartsWith("Error")) return NotFound(message);
            return Ok(message);
        }
    }
}