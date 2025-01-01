using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
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
            IEnumerable<object> reportData;

            switch (entity.ToLower())
            {
                case "clients":
                    reportData = _context.Clients
                        .Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
                        .OrderBy(C => C.CreatedAt)
                        .Select(c => new { c.ClientId, c.Name, c.CreatedAt })
                        .ToList();
                    break;

                case "suppliers":
                    reportData = _context.Suppliers
                        .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
                        .OrderBy(C => C.CreatedAt)
                        .Select(s => new { s.SupplierId, s.Name, s.CreatedAt })
                        .ToList();
                    break;

                case "warehouses":
                    reportData = _context.Warehouses
                        .Where(w => w.CreatedAt >= fromDate && w.CreatedAt <= toDate && (warehouseId == null ? true : w.WarehouseId == warehouseId))
                        .OrderBy(C => C.CreatedAt)
                        .Select(w => new { w.WarehouseId, w.Name, w.CreatedAt })
                        .ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid entity type for reporting.");
            }

            if (reportData == null || reportData.Count() == 0)
            {
                return reportData;
            }
            
            // schrijft naar een bestand toe
            WriteReportToFile(entity, fromDate, toDate, warehouseId, reportData);

            return reportData;
        }

        // de method. Is te vinden bij de volgende path: GitHub\Processing-and-Tools-Team-2\CargoHubRefactor\bin\Debug\Reports
        private void WriteReportToFile(string entity, DateTime fromDate, DateTime toDate, int? warehouseId, IEnumerable<object> reportData)
        {
            string fileName = $"{entity}_Report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.txt";
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

                foreach (var record in reportData)
                {
                    writer.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
                }

                writer.WriteLine("--------------------------------------------------");
                writer.WriteLine($"Generated on: {DateTime.Now}");
            }
        }

    }
}
