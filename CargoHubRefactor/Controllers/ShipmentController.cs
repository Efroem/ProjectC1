using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateShipmentStatus(int id, [FromBody] ShipmentStatusUpdateRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Status))
        {
            return BadRequest("Invalid request. Status is required.");
        }

        var result = await _shipmentService.UpdateShipmentStatusAsync(id, request.Status);

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

    [HttpPost("split-order")]
    public async Task<IActionResult> SplitOrder([FromBody] SplitOrderRequest request)
    {
        if (request == null || request.ItemsToSplit == null || !request.ItemsToSplit.Any())
            return BadRequest("Invalid split request. ItemsToSplit cannot be null or empty.");

        var result = await _shipmentService.SplitOrderIntoShipmentsAsync(request.OrderId, request.ItemsToSplit);

        if (result.StartsWith("Error"))
            return BadRequest(result);

        return Ok(result);
    }


}