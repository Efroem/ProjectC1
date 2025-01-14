using CargoHubRefactor.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [ApiController]
    [Route("api/v1/reporting")]
    public class ReportingController : ControllerBase
    {
        private readonly ReportingService _reportingService;
        private readonly string _reportDirectory;

        public ReportingController(ReportingService reportingService)
        {
            _reportingService = reportingService;
            _reportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            Directory.CreateDirectory(_reportDirectory);
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
            if (entity.Contains("..") || entity.Contains("/") || entity.Contains("\\"))
                return BadRequest("Invalid entity value.");

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

        [HttpGet("download")]
        public IActionResult DownloadReport(string entity, DateTime fromDate, DateTime toDate, int? warehouseId)
        {
            // Generate the report
            var reportData = _reportingService.GenerateReport(entity, fromDate, toDate, warehouseId);

            // Construct the file name
            string fileName = $"{entity}_Report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}{(warehouseId.HasValue ? "_Id_" + warehouseId : "")}.csv";

            // File path (replace this with the actual path where the report is stored)
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Report file not found.");
            }

            // Return the file as a download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/csv", fileName);
        }
    }
}
