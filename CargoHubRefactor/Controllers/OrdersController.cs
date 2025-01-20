//ORDERSCONTROLLER
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CargoHubRefactor.Controllers
{

    [ServiceFilter(typeof(Filters))]
[ApiController]
[Route("api/v1/Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] Order order)
    {
        if (order == null)
        {
            return BadRequest("Order data cannot be null");
        }

        if (string.IsNullOrEmpty(order.Reference))
        {
            return BadRequest("Order reference cannot be empty");
        }

        if (order.SourceId <= 0)
        {
            return BadRequest("'SourceId' must be a valid positive integer");
        }

        if (order.WarehouseId <= 0)
        {
            return BadRequest("'WarehouseId' must be a valid positive integer");
        }

        if (order.ShipTo < 0)
        {
            return BadRequest("'ShipTo' must be a valid positive integer");
        }

        if (order.BillTo < 0)
        {
            return BadRequest("'BillTo' must be a valid positive integer");
        }

        if (order.TotalDiscount < 0)
        {
            return BadRequest("'TotalDiscount' cannot be negative");
        }

        if (order.TotalTax < 0)
        {
            return BadRequest("'TotalTax' cannot be negative");
        }

        if (order.TotalSurcharge < 0)
        {
            return BadRequest("'TotalSurcharge' cannot be negative");
        }

        if (order.RequestDate < order.OrderDate)
        {
            return BadRequest("'RequestDate' cannot be earlier than 'OrderDate'");
        }

        // Call AddOrderAsync with updated arguments
        var createdOrder = await _orderService.AddOrderAsync(
            order.SourceId,
            order.OrderDate,
            order.RequestDate,
            order.Reference,
            order.ReferenceExtra,
            order.Notes,
            order.ShippingNotes,
            order.PickingNotes,
            order.WarehouseId,
            order.ShipTo,
            order.BillTo,
            order.ShipmentId,
            order.TotalDiscount,
            order.TotalTax,
            order.TotalSurcharge,
            order.OrderItems);

        return Ok(createdOrder);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        if (order == null)
        {
            return BadRequest("Order data cannot be null");
        }

        if (string.IsNullOrEmpty(order.Reference))
        {
            return BadRequest("'Reference' is required");
        }

        if (order.SourceId <= 0)
        {
            return BadRequest("'SourceId' must be a valid positive integer");
        }

        if (order.WarehouseId <= 0)
        {
            return BadRequest("'WarehouseId' must be a valid positive integer");
        }

        if (order.ShipTo < 0)
        {
            return BadRequest("'ShipTo' must be a valid positive integer");
        }

        if (order.BillTo < 0)
        {
            return BadRequest("'BillTo' must be a valid positive integer");
        }

        if (order.TotalDiscount < 0)
        {
            return BadRequest("'TotalDiscount' cannot be negative");
        }

        if (order.TotalTax < 0)
        {
            return BadRequest("'TotalTax' cannot be negative");
        }

        if (order.TotalSurcharge < 0)
        {
            return BadRequest("'TotalSurcharge' cannot be negative");
        }

        if (order.RequestDate < order.OrderDate)
        {
            return BadRequest("'RequestDate' cannot be earlier than 'OrderDate'");
        }

        // Validate and restrict OrderStatus updates
        if (!string.IsNullOrEmpty(order.OrderStatus) && order.OrderStatus != "InProgress" && order.OrderStatus != "Delivered")
        {
            return BadRequest("OrderStatus can only be updated to 'InProgress' or 'Delivered'.");
        }

        var updatedOrder = await _orderService.UpdateOrderAsync(
            id,
            order.SourceId,
            order.OrderDate,
            order.RequestDate,
            order.Reference,
            order.ReferenceExtra,
            order.OrderStatus ?? "Pending",
            order.Notes,
            order.ShippingNotes,
            order.PickingNotes,
            order.WarehouseId,
            order.ShipTo,
            order.BillTo,
            order.ShipmentId,
            order.TotalDiscount,
            order.TotalTax,
            order.TotalSurcharge);

        if (updatedOrder == null)
        {
            return NotFound($"Order with ID: {id} was not found");
        }

        return Ok(updatedOrder);
    }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success)
            {
                return NotFound($"Order with ID: {id} was not found");
            }

            return Ok($"Order with ID: {id} successfully deleted");
        }

        [HttpDelete("{id}/test")]
        public async Task<IActionResult> SoftDeleteOrder(int id)
        {
            var success = await _orderService.SoftDeleteOrderAsync(id);
            if (!success)
            {
                return NotFound($"Order with ID: {id} was not found");
            }

            return Ok($"Order with ID: {id} successfully soft deleted");
        }

        [HttpGet("{orderId}/locations")]
        public async Task<ActionResult<Dictionary<string, Dictionary<int, int>>>> GetOrderItemLocations(int orderId)
        {
            var groupedLocations = await _orderService.GetLocationsForOrderItemsAsync(orderId);

            if (groupedLocations == null || !groupedLocations.Any())
                return NotFound("Locations not found for the order.");

            return Ok(groupedLocations);
        }

    }
}