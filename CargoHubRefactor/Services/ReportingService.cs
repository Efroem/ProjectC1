using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CargoHubRefactor.Services
{
    public class ReportingService
    {
        private readonly CargoHubDbContext _context;
        private readonly string _reportDirectory;

        public ReportingService(CargoHubDbContext context)
        {
            _context = context;
            _reportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            Directory.CreateDirectory(_reportDirectory);
        }

        public IEnumerable<object> GenerateReport(string entity, DateTime fromDate, DateTime toDate, int? warehouseId)
        {
            // Validate date range
            if (fromDate > toDate)
            {
                throw new ArgumentException("The fromDate cannot be later than toDate.");
            }

            IEnumerable<object> reportData;

            switch (entity.ToLower())
            {
                case "clients":
                    reportData = _context.Clients
                        .Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new { c.ClientId, c.Name, c.CreatedAt })
                        .ToList();
                    break;

                case "suppliers":
                    reportData = _context.Suppliers
                        .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
                        .OrderBy(s => s.CreatedAt)
                        .Select(s => new { s.SupplierId, s.Name, s.CreatedAt })
                        .ToList();
                    break;

                case "warehouses":
                    reportData = _context.Warehouses
                        .Where(w => w.CreatedAt >= fromDate && w.CreatedAt <= toDate && (warehouseId == null || w.WarehouseId == warehouseId))
                        .OrderBy(w => w.CreatedAt)
                        .Select(w => new { w.WarehouseId, w.Name, w.CreatedAt })
                        .ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid entity type for reporting.");
            }

            if (reportData == null || !reportData.Any())
            {
                return reportData;
            }

            // Write the report to a file
            WriteReportToFile(entity, fromDate, toDate, warehouseId, reportData);

            return reportData;
        }

        private void WriteReportToFile(string entity, DateTime fromDate, DateTime toDate, int? warehouseId, IEnumerable<object> reportData)
        {
            // Construct the file name dynamically
            string fileName = $"{entity}_Report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}{(warehouseId.HasValue ? "_Id_" + warehouseId : "")}.csv";
            string filePath = Path.Combine(_reportDirectory, fileName);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine($"Report for entity: {entity}");
                writer.WriteLine($"Date range: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}");
                if (warehouseId.HasValue)
                {
                    writer.WriteLine($"Warehouse ID: {warehouseId}");
                }
                writer.WriteLine("--------------------------------------------------");

                // Handle entity-specific headers and data
                switch (entity.ToLower())
                {
                    case "clients":
                        writer.WriteLine("ClientId,Name,CreatedAt");
                        foreach (var record in reportData)
                        {
                            var client = (dynamic)record;
                            writer.WriteLine($"{client.ClientId},{client.Name},{client.CreatedAt:yyyy-MM-dd}");
                        }
                        break;

                    case "suppliers":
                        writer.WriteLine("SupplierId,Name,CreatedAt");
                        foreach (var record in reportData)
                        {
                            var supplier = (dynamic)record;
                            writer.WriteLine($"{supplier.SupplierId},{supplier.Name},{supplier.CreatedAt:yyyy-MM-dd}");
                        }
                        break;

                    case "warehouses":
                        writer.WriteLine("WarehouseId,Name,CreatedAt");
                        foreach (var record in reportData)
                        {
                            var warehouse = (dynamic)record;
                            writer.WriteLine($"{warehouse.WarehouseId},{warehouse.Name},{warehouse.CreatedAt:yyyy-MM-dd}");
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported entity: {entity}");
                }

                writer.WriteLine("--------------------------------------------------");
                writer.WriteLine($"Generated on: {DateTime.Now}");
            }
        }
    }
}
