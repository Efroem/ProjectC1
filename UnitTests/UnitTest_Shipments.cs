namespace UnitTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoHubRefactor;

/*
WHEN MAKING A NEW UNIT TEST FILE FOR AN ENDPOINT. COPY OVER Setup AND SeedDatabase
*/

[TestClass]
public class UnitTest_Shipments
{
    private CargoHubDbContext _dbContext;
    private ShipmentService _shipmentService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        // Set up an in-memory database
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase_Shipments")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        _shipmentService = new ShipmentService(_dbContext);

        // Seed test data
        SeedDatabase(_dbContext);
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
            OrderIds = new List<int> { 1 },
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
            OrderIds = new List<int> { 2 },
            OrderDate = DateTime.UtcNow.AddDays(-7),
            RequestDate = DateTime.UtcNow.AddDays(-4),
            ShipmentDate = DateTime.UtcNow.AddDays(-2),
            ShipmentType = "Standard",
            ShipmentStatus = "Pending",
            Notes = "Another test shipment",
            CarrierCode = "FedEx",
            CarrierDescription = "FedEx Standard",
            ServiceCode = "STD",
            PaymentType = "Collect",
            TransferMode = "Truck",
            TotalPackageCount = 5,
            TotalPackageWeight = 25.0,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-7)
        });

        // Seed Orders
        context.Orders.Add(new Order
        {
            Id = 1,
            SourceId = 1,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            RequestDate = DateTime.UtcNow.AddDays(-3),
            Reference = "Order1",
            OrderStatus = "Complete",
            ShipmentId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-6),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        });

        context.Orders.Add(new Order
        {
            Id = 2,
            SourceId = 2,
            OrderDate = DateTime.UtcNow.AddDays(-7),
            RequestDate = DateTime.UtcNow.AddDays(-4),
            Reference = "Order2",
            OrderStatus = "Pending",
            ShipmentId = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            UpdatedAt = DateTime.UtcNow.AddDays(-7)
        });

        context.SaveChanges();
    }

    [TestMethod]
    public async Task TestGetAllShipments()
    {
        // Act
        var shipments = await _shipmentService.GetAllShipmentsAsync();

        // Assert
        Assert.IsNotNull(shipments);
        Assert.IsTrue(shipments.Count > 0);
    }

    [TestMethod]
    [DataRow(1, true)] // Test with an existing shipment ID
    [DataRow(999, false)] // Test with a non-existent shipment ID
    public async Task TestGetShipmentById(int shipmentId, bool shouldExist)
    {
        // Act
        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);

        // Assert
        Assert.AreEqual(shipment != null, shouldExist);
    }

    [TestMethod]
    public async Task TestAddShipment()
    {
        // Arrange
        var newShipment = new Shipment
        {
            ShipmentId = 3,
            SourceId = 3,
            OrderIds = new List<int> { 3 },
            OrderDate = DateTime.UtcNow,
            RequestDate = DateTime.UtcNow,
            ShipmentDate = DateTime.UtcNow,
            ShipmentType = "Standard",
            ShipmentStatus = "Pending",
            Notes = "New shipment",
            CarrierCode = "DHL",
            CarrierDescription = "DHL Standard",
            ServiceCode = "STD",
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
            OrderIds = new List<int> { 1, 2 }, // Add another order
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