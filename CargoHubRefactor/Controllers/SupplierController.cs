using Microsoft.AspNetCore.Mvc;
using Services;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/suppliers")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

         [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound($"Supplier with ID: {id} not found.");
            }
            return Ok(supplier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            if (suppliers == null)
            {
                return NotFound($"No suppliers found.");
            }
            return Ok(suppliers);
        }

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show suppliers with a limit below 1.");
            }

            var suppliers = await _supplierService.GetAllSuppliersAsync(limit);
            if (suppliers == null || !suppliers.Any())
            {
                return NotFound("No suppliers found.");
            }

            return Ok(suppliers);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot supplier with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var supplier = await _supplierService.GetSuppliersPagedAsync(limit, page);
            if (supplier == null || !supplier.Any())
            {
                return NotFound("No supplier found.");
            }

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int lowestAvailableId = await _supplierService.GetLowestAvailableSupplierIdAsync();
            supplier.SupplierId = lowestAvailableId;

            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.SupplierId }, createdSupplier);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return BadRequest($"Supplier ID mismatch. Expected {id}, got {supplier.SupplierId}.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _supplierService.UpdateSupplierAsync(id, supplier);
            if (!updated)
                return NotFound();

            return Ok(supplier);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var deleted = await _supplierService.DeleteSupplierAsync(id);
            if (!deleted)
            {
                return NotFound($"Supplier with ID: {id} not found.");

            }
            return Ok($"Supplier with ID: {id} successfully deleted");
        }

        [HttpDelete("{id}/test")]
        public async Task<IActionResult> SoftDeleteSupplier(int id)
        {
            var deleted = await _supplierService.SoftDeleteSupplierAsync(id);
            if (!deleted)
            {
                return NotFound($"Supplier with ID: {id} not found.");

            }
            return Ok($"Supplier with ID: {id} successfully soft deleted");
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllSuppliers()
        {
            var deleted = await _supplierService.DeleteAllSuppliersAsync();
            if (!deleted)
                return NotFound("No suppliers found to delete.");

            return Ok("All Suppliers deleted succesfully");
        }



    }
}
