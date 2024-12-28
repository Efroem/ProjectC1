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
            // Schrijf naar een tekstbestand
            WriteReportToFile(entity, fromDate, toDate, warehouseId, reportData);

            return reportData;
        }

        private void WriteReportToFile(string entity, DateTime fromDate, DateTime toDate, int? warehouseId, IEnumerable<object> reportData)
        {
            string fileName = $"{entity}_Report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.txt";
            string filePath = Path.Combine(_reportDirectory, fileName);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Maak de samenvatting in één regel
            string reportLine = $"Entity: {entity}, " +
                                $"DateRange: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}, " +
                                $"WarehouseId: {(warehouseId.HasValue ? warehouseId.ToString() : "N/A")}, " +
                                $"Records: [{string.Join(", ", JsonSerializer.Serialize(reportData, options))}]";

            // Schrijf de regel naar het bestand
            File.WriteAllText(filePath, reportLine);
        }

    }
}
