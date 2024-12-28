using CargoHubRefactor.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CargoHubRefactor.Controllers
{
    [ApiController]
    [Route("api/v1/reporting")]
    public class ReportingController : ControllerBase
    {
        private readonly ReportingService _reportingService;

        public ReportingController(ReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet]
        public IActionResult GetReport(
            [FromQuery] string entity,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? warehouseId)
        {
            if (string.IsNullOrEmpty(entity))
                return BadRequest("Entity is required.");
            if (!fromDate.HasValue || !toDate.HasValue)
                return BadRequest("fromDate and toDate are required.");

            try
            {
                var report = _reportingService.GenerateReport(entity, fromDate.Value, toDate.Value, warehouseId);
                if (report == null || report.Count() == 0)
                {
                    return BadRequest($"No report data for the applied filters");
                }
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
