namespace UnitTests;

using CargoHubRefactor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class UnitTest_Reporting
{
    private CargoHubDbContext _dbContext;
    private ReportingService _reportingService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        // Initialize in-memory database
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);

        // Seed test data
        SeedDatabase(_dbContext);

        // Initialize ReportingService
        _reportingService = new ReportingService(_dbContext);
    }

    private void SeedDatabase(CargoHubDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Clients
        context.Clients.AddRange(new List<Client>
        {
            new Client 
            { 
                ClientId = 1, 
                Name = "John Doe", 
                Address = "123 Main Street", 
                City = "Springfield", 
                ZipCode = "12345", 
                Province = "Province1", 
                Country = "Country1", 
                ContactName = "John Contact", 
                ContactPhone = "123-456-7890", 
                ContactEmail = "john@example.com", 
                CreatedAt = new DateTime(2023, 01, 01), 
                UpdatedAt = DateTime.UtcNow 
            },
            new Client 
            { 
                ClientId = 2, 
                Name = "Jane Doe", 
                Address = "456 Elm Street", 
                City = "Shelbyville", 
                ZipCode = "67890", 
                Province = "Province2", 
                Country = "Country2", 
                ContactName = "Jane Contact", 
                ContactPhone = "098-765-4321", 
                ContactEmail = "jane@example.com", 
                CreatedAt = new DateTime(2023, 05, 01), 
                UpdatedAt = DateTime.UtcNow 
            }
        });

        context.SaveChanges();
    }

    [TestMethod]
    public void GenerateReport_ShouldReturnClientsData_WhenEntityIsClients()
    {
        // Act
        var result = _reportingService.GenerateReport(
            "clients",
            new DateTime(2023, 01, 01),
            new DateTime(2023, 12, 31),
            null
        );

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void GenerateReport_ShouldThrowArgumentException_WhenEntityIsInvalid()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _reportingService.GenerateReport(
                "invalidEntity",
                DateTime.Now,
                DateTime.Now,
                null
            )
        );
    }

    [TestMethod]
    public void GenerateReport_ShouldReturnEmpty_WhenNoDataMatchesFilters()
    {
        // Act
        var result = _reportingService.GenerateReport(
            "clients",
            new DateTime(2024, 01, 01),
            new DateTime(2024, 12, 31),
            null
        );

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }
}
