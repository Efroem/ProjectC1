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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID: {id} was not found");
            }
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders were found");
            }
            return Ok(orders);
        }
        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetOrders(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show orders with a limit below 1.");
            }

            var orders = await _orderService.GetOrdersAsync(limit);
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(orders);
        }

        [HttpGet("{id}/TotalPrice")]
        public async Task<IActionResult> GetOrderPriceTotal(int id) 
        {
            var totalPrice = await _orderService.GetOrderPriceTotalAsync(id);
            if (totalPrice <= 0) {
                return BadRequest("Error: Order does not exist or Order contains invalid prices");
            }
            return Ok($"Total Price for Order {id}: €\n{totalPrice:F2}");
        }

        [HttpGet("{id}/TotalWeight")]
        public async Task<IActionResult> GetOrderWeightTotal(int id) 
        {
            var totalWeight = await _orderService.GetOrderWeightTotalAsync(id);
            if (totalWeight <= 0) {
                return BadRequest("Error: Order does not exist or Order contains invalid weights");
            }
            return Ok($"Total Weight for Order {id}:  \n{totalWeight:F2} KG");
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

            if (string.IsNullOrEmpty(order.OrderStatus))
            {
                return BadRequest("Order status cannot be empty");
            }

            if (order.SourceId <= 0)
            {
                return BadRequest("SourceId must be a valid positive integer");
            }

            if (order.WarehouseId <= 0)
            {
                return BadRequest("WarehouseId must be a valid positive integer");
            }

            if (order.ShipTo < 0)
            {
                return BadRequest("ShipTo must be a valid positive integer");
            }

            if (order.BillTo < 0)
            {
                return BadRequest("BillTo must be a valid positive integer");
            }

            if (order.TotalAmount < 0)
            {
                return BadRequest("TotalAmount must be a positive number");
            }

            if (order.TotalDiscount < 0)
            {
                return BadRequest("TotalDiscount cannot be negative");
            }

            if (order.TotalTax < 0)
            {
                return BadRequest("TotalTax cannot be negative");
            }

            if (order.TotalSurcharge < 0)
            {
                return BadRequest("TotalSurcharge cannot be negative");
            }

            if (order.RequestDate < order.OrderDate)
            {
                return BadRequest("orderDate cannot be earlier than OrderDate");
            }
            var CreatedOrder = await _orderService.AddOrderAsync(
                order.SourceId,
                order.OrderDate,
                order.RequestDate,
                order.Reference,
                order.ReferenceExtra,
                order.OrderStatus,
                order.Notes,
                order.ShippingNotes,
                order.PickingNotes,
                order.WarehouseId,
                order.ShipTo,
                order.BillTo,
                order.ShipmentId,
                order.TotalAmount,
                order.TotalDiscount,
                order.TotalTax,
                order.TotalSurcharge,
                order.OrderItems);

            return Ok(CreatedOrder);
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
                return BadRequest("Reference is required");
            }

            if (string.IsNullOrEmpty(order.OrderStatus))
            {
                return BadRequest("Order status is required");
            }

            if (order.SourceId <= 0)
            {
                return BadRequest("SourceId must be a valid positive integer");
            }

            if (order.WarehouseId <= 0)
            {
                return BadRequest("WarehouseId must be a valid positive integer");
            }

            if (order.ShipTo < 0)
            {
                return BadRequest("ShipTo must be a valid positive integer");
            }

            if (order.BillTo < 0)
            {
                return BadRequest("BillTo must be a valid positive integer");
            }

            if (order.TotalAmount < 0)
            {
                return BadRequest("TotalAmount must be a positive number");
            }

            if (order.TotalDiscount < 0)
            {
                return BadRequest("TotalDiscount cannot be negative");
            }

            if (order.TotalTax < 0)
            {
                return BadRequest("TotalTax cannot be negative");
            }

            if (order.TotalSurcharge < 0)
            {
                return BadRequest("TotalSurcharge cannot be negative");
            }

            if (order.RequestDate < order.OrderDate)
            {
                return BadRequest("RequestDate cannot be earlier than OrderDate");
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(
                id,
                order.SourceId,
                order.OrderDate,
                order.RequestDate,
                order.Reference,
                order.ReferenceExtra,
                order.OrderStatus,
                order.Notes,
                order.ShippingNotes,
                order.PickingNotes,
                order.WarehouseId,
                order.ShipTo,
                order.BillTo,
                order.ShipmentId,
                order.TotalAmount,
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
        public async Task<ActionResult<Dictionary<string, List<Location>>>> GetOrderItemLocations(int orderId)
        {
            var groupedLocations = await _orderService.GetLocationsForOrderItemsAsync(orderId);

            if (groupedLocations == null || !groupedLocations.Any())
                return NotFound("Locations not found for the order.");

            return Ok(groupedLocations);
        }

    }
}