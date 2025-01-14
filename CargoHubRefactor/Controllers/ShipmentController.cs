using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
[ServiceFilter(typeof(Filters))]
[Route("api/v1/shipments")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Shipment>>> GetAllShipments()
    {
        return Ok(await _shipmentService.GetAllShipmentsAsync());
    }

    [HttpGet("limit/{limit}")]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllShipments(int limit)
    {
        if (limit <= 0)
        {
            return BadRequest("Cannot show shipments with a limit below 1.");
        }

        var shipments = await _shipmentService.GetAllShipmentsAsync(limit);
        if (shipments == null || !shipments.Any())
        {
            return NotFound("No shipments found.");
        }

        return Ok(shipments);
    }

    [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetShipmentsPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show shipments with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var shipments = await _shipmentService.GetShipmentsPagedAsync(limit, page);
            if (shipments == null || !shipments.Any())
            {
                return NotFound("No shipments found.");
            }

            return Ok(shipments);
        }


    [HttpGet("{id}")]
    public async Task<ActionResult<Shipment>> GetShipmentById(int id)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
        if (shipment == null)
        {
            return NotFound("Error: Shipment not found.");
        }
        return Ok(shipment);
    }

    [HttpPost]
    public async Task<ActionResult> AddShipment([FromBody] Shipment shipment)
    {
        var result = await _shipmentService.AddShipmentAsync(shipment);
        if (result.message.StartsWith("Error"))
        {
            return BadRequest(result.message);
        }
        return Ok(result.shipment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
    {
        var result = await _shipmentService.UpdateShipmentAsync(id, shipment);
        if (result.StartsWith("Error"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{id}/items")]
    public async Task<ActionResult<List<ShipmentItem>>> GetShipmentItems(int id)
    {
        var shipmentItems = await _shipmentService.GetShipmentItemsAsync(id);

        if (shipmentItems == null || shipmentItems.Count == 0)
        {
            return NotFound("No items found for the given shipment.");
        }
        return Ok(shipmentItems);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShipment(int id)
    {
        var result = await _shipmentService.DeleteShipmentAsync(id);
        if (result.StartsWith("Error"))
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}