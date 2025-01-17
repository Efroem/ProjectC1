//SHIPMENTS CONTROLLER
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
        if (result.shipment == null)
        {
            return BadRequest(result.message);
        }
        return Ok(result.shipment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
    {
        var result = await _shipmentService.UpdateShipmentAsync(id, shipment);
        if (!result.Contains("Shipment successfully updated."))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateShipmentStatus(int id, [FromBody] ShipmentStatusUpdateRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Status))
        {
            return BadRequest("Invalid request. Status is required.");
        }

        var result = await _shipmentService.UpdateShipmentStatusAsync(id, request.Status);

        if (!result.Contains("successfully updated"))
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
        if (!result.Contains("successfully deleted"))
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id}/test")]
    public async Task<ActionResult> SoftDeleteShipment(int id)
    {
        var result = await _shipmentService.DeleteShipmentAsync(id);
        if (!result.Contains("successfully soft deleted"))
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost("split-order")]
    public async Task<IActionResult> SplitOrder([FromBody] SplitOrderRequest request)
    {
        if (request == null || request.ItemsToSplit == null || !request.ItemsToSplit.Any())
            return BadRequest("Invalid split request. ItemsToSplit cannot be null or empty.");

        var result = await _shipmentService.SplitOrderIntoShipmentsAsync(request.OrderId, request.ItemsToSplit);

        if (!result.Contains("Successfully split order"))
            return BadRequest(result);

        return Ok(result);
    }


}