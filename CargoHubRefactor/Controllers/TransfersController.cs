using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CargoHubRefactor.Controllers{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        // GET: api/v1/Transfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetTransfers()
        {
            var transfers = await _transferService.GetAllTransfersAsync();
            return Ok(transfers);
        }

        // GET: api/v1/Transfers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transfer>> GetTransferById(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null)
            {
                return NotFound("Transfer not found.");
            }
            return Ok(transfer);
        }

        // POST: api/v1/Transfers
        [HttpPost]
        public async Task<IActionResult> AddTransfer([FromBody] Transfer transfer)
        {
            var (message, createdTransfer) = await _transferService.AddTransferAsync(new Transfer
            {
                Reference = transfer.Reference,
                TransferFrom = transfer.TransferFrom,
                TransferTo = transfer.TransferTo,
                TransferStatus = transfer.TransferStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createdTransfer == null)
            {
                return BadRequest(message);
            }

            return CreatedAtAction(nameof(GetTransferById), new { id = createdTransfer.TransferId }, createdTransfer);
        }


        // PUT: api/v1/Transfers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransfer(int id, Transfer transfer)
        {
            // Ensure the route ID matches the transfer ID
            if (id != transfer.TransferId)
            {
                return BadRequest("Transfer ID in the URL does not match the body.");
            }

            var message = await _transferService.UpdateTransferAsync(id, transfer);
            if (message.StartsWith("Error"))
            {
                return BadRequest(message);
            }

            return Ok(message);
        }

        // DELETE: api/v1/Transfers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransfer(int id)
        {
            var message = await _transferService.DeleteTransferAsync(id);
            if (message.StartsWith("Error"))
            {
                return NotFound(message);
            }

            return Ok(message);
        }
    }
}