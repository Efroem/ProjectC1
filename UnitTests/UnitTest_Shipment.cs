//UNITTEST SHIPMENTS
namespace UnitTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

/*
WHEN MAKING A NEW UNIT TEST FILE FOR AN ENDPOINT. COPY OVER Setup AND SeedDatabase
*/


[TestClass]
public class UnitTest_Shipment
{
    private CargoHubDbContext _dbContext;
    private ShipmentService _shipmentService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        SeedDatabase(_dbContext);
        var orderService = new OrderService(_dbContext); // Assuming OrderService implements IOrderService
        _shipmentService = new ShipmentService(_dbContext, orderService);
    }
    private void SeedDatabase(CargoHubDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Shipments
        context.Shipments.Add(new Shipment
        {
            ShipmentId = 1,
            SourceId = 1,
            OrderId = "4",
            OrderDate = DateTime.UtcNow.AddDays(-5),
            RequestDate = DateTime.UtcNow.AddDays(-3),
            ShipmentDate = DateTime.UtcNow.AddDays(-1),
            ShipmentType = "Express",
            ShipmentStatus = "Shipped",
            Notes = "Test shipment",
            CarrierCode = "UPS",
            CarrierDescription = "UPS Express",
            ServiceCode = "EXP",
            PaymentType = "Prepaid",
            TransferMode = "Air",
            TotalPackageCount = 2,
            TotalPackageWeight = 10.5,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        });

        context.Shipments.Add(new Shipment
        {
            ShipmentId = 2,
            SourceId = 2,
            OrderId = "1,2,3",
            OrderDate = DateTime.UtcNow.AddDays(-7),
            RequestDate = DateTime.UtcNow.AddDays(-4),
            ShipmentDate = DateTime.UtcNow.AddDays(-2),
            ShipmentType = "Standard",
            ShipmentStatus = "Pending",
            Notes = "Another test shipment",
            CarrierCode = "FedEx",
            CarrierDescription = "FedEx Standard",
            ServiceCode = "STP",
            PaymentType = "Collect",
            TransferMode = "Truck",
            TotalPackageCount = 5,
            TotalPackageWeight = 25.0,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-7)
        });

        context.SaveChanges();
    }


    [TestMethod]
    public async Task TestAddShipment()
    {
        // Arrange
        var newShipment = new Shipment
        {
            ShipmentId = 3,
            SourceId = 3,
            OrderId = "15,27",
            OrderDate = DateTime.UtcNow,
            RequestDate = DateTime.UtcNow,
            ShipmentDate = DateTime.UtcNow,
            ShipmentType = "Standard",
            ShipmentStatus = "Pending",
            Notes = "New shipment",
            CarrierCode = "DHL",
            CarrierDescription = "DHL Standard",
            ServiceCode = "STF",
            PaymentType = "Prepaid",
            TransferMode = "Truck",
            TotalPackageCount = 3,
            TotalPackageWeight = 15.0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _shipmentService.AddShipmentAsync(newShipment);

        // Assert
        Assert.IsNotNull(result.shipment);
        Assert.AreEqual(result.message, "Shipment successfully created.");
    }

    [TestMethod]
    public async Task TestUpdateShipment()
    {
        // Arrange
        var updatedShipment = new Shipment
        {
            ShipmentId = 1,
            SourceId = 1,
            OrderId = "1,2,3,4",
            OrderDate = DateTime.UtcNow,
            RequestDate = DateTime.UtcNow,
            ShipmentDate = DateTime.UtcNow,
            ShipmentType = "Express",
            ShipmentStatus = "Delivered",
            Notes = "Updated shipment",
            CarrierCode = "UPS",
            CarrierDescription = "UPS Updated",
            ServiceCode = "EXP",
            PaymentType = "Prepaid",
            TransferMode = "Air",
            TotalPackageCount = 3,
            TotalPackageWeight = 12.5,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _shipmentService.UpdateShipmentAsync(1, updatedShipment);

        // Assert
        Assert.AreEqual(result, "Shipment successfully updated.");
    }

    [TestMethod]
    [DataRow(1, true)] // Test with an existing shipment ID
    [DataRow(999, false)] // Test with a non-existent shipment ID
    public async Task TestDeleteShipment(int shipmentId, bool shouldDelete)
    {
        // Act
        var result = await _shipmentService.DeleteShipmentAsync(shipmentId);

        // Assert
        Assert.AreEqual(result.StartsWith("Shipment successfully deleted."), shouldDelete);
    }
}

