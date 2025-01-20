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

        context.ItemGroups.Add(new ItemGroup
        {
            GroupId = 1,  // Ensure unique GroupId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemGroups.Add(new ItemGroup
        {
            GroupId = 2,  // Ensure unique GroupId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed ItemTypes with unique IDs
        context.ItemTypes.Add(new ItemType
        {
            TypeId = 1,  // Ensure unique TypeId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemTypes.Add(new ItemType
        {
            TypeId = 2,  // Ensure unique TypeId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed ItemLines with unique IDs
        context.ItemLines.Add(new ItemLine
        {
            LineId = 1,  // Ensure unique LineId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemLines.Add(new ItemLine
        {
            LineId = 2,  // Ensure unique LineId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Items with unique codes and references
        context.Items.Add(new Item
        {
            Uid = "P000001",  // Unique Item Uid
            Code = "Dummy",
            Description = "dummy",
            ShortDescription = "dummy",
            UpcCode = "null",
            ModelNumber = "null",
            CommodityCode = "null",
            ItemLine = 1,  // Reference the unique ItemLine ID
            ItemGroup = 1,  // Reference the unique ItemGroup ID
            ItemType = 1,  // Reference the unique ItemType ID
            UnitPurchaseQuantity = 1,
            UnitOrderQuantity = 1,
            PackOrderQuantity = 1,
            SupplierId = 1,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Items.Add(new Item
        {
            Uid = "P000002",  // Unique Item Uid
            Code = "Dummy2",
            Description = "dummy2",
            ShortDescription = "dummy2",
            UpcCode = "null",
            ModelNumber = "null",
            CommodityCode = "null",
            ItemLine = 2,  // Reference the unique ItemLine ID
            ItemGroup = 2,  // Reference the unique ItemGroup ID
            ItemType = 2,  // Reference the unique ItemType ID
            UnitPurchaseQuantity = 1,
            UnitOrderQuantity = 1,
            PackOrderQuantity = 1,
            SupplierId = 2,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Warehouses.Add(new Warehouse
        {
            WarehouseId = 1, // Ensure unique WarehouseId
            Code = "WH001",  // Unique code for the warehouse
            Name = "Main Warehouse",
            Address = "123 Warehouse St.",
            Zip = "12345",
            City = "Sample City",
            Province = "Sample Province",
            Country = "Sample Country",
            ContactName = "John Doe",
            ContactPhone = "555-1234",
            ContactEmail = "johndoe@example.com",
            RestrictedClassificationsList = new List<string> { "DummyRestricted" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Locations.Add(new Location
        {
            LocationId = 1,
            Name = "Row: A, Rack: 1, Shelf: 1",
            Code = "LOC001",
            WarehouseId = 1,
            ItemAmounts = new Dictionary<string, int>{
                {"P000001", 10},
                {"P000002", 10}
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Locations.Add(new Location
        {
            LocationId = 2,
            Name = "Row: B, Rack: 2, Shelf: 2",
            Code = "LOC002",
            ItemAmounts = new Dictionary<string, int>{
                {"P000001", 10},
                {"P000002", 10}
            },
            WarehouseId = 1,
            MaxHeight = 100,
            MaxWidth = 20,
            MaxDepth = 20,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Locations.Add(new Location
        {
            LocationId = 3,
            Name = "Row: C, Rack: 3, Shelf: 3",
            Code = "LOC002",
            WarehouseId = 2,
            ItemAmounts = new Dictionary<string, int> { },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Inventories.Add(new Inventory
        {
            InventoryId = 1,  // Ensure unique InventoryId
            ItemId = "P000001",  // Reference the unique ItemId
            Description = "dummy",
            ItemReference = "dummy",
            TotalOnHand = 100,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 20,
            LocationsList = new List<int> { 1, 2 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Inventories.Add(new Inventory
        {
            InventoryId = 2,  // Ensure unique InventoryId
            ItemId = "P000002",  // Reference the unique ItemId
            Description = "dummy2",
            ItemReference = "dummy2",
            TotalOnHand = 100,
            TotalExpected = 1,
            TotalOrdered = 1,
            LocationsList = new List<int> { 1, 2 },
            TotalAllocated = 1,
            TotalAvailable = 20,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Orders.Add(new Order
        {
            Id = 1,
            SourceId = 1,
            OrderDate = DateTime.UtcNow.AddDays(-7),
            RequestDate = DateTime.UtcNow.AddDays(-3),
            Reference = "ORD-12345",
            ReferenceExtra = "Special handling required",
            OrderStatus = "Processing",
            Notes = "Customer prefers expedited shipping.",
            ShippingNotes = "Fragile items. Handle with care.",
            PickingNotes = "Verify quantities before packing.",
            WarehouseId = 5,
            ShipTo = 2001,
            BillTo = 2002,
            ShipmentId = 3001,
            TotalAmount = 500.75,
            TotalDiscount = 50.00,
            TotalTax = 25.50,
            TotalSurcharge = 10.00,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.OrderItems.Add(new OrderItem
        {
            Id = 1,
            OrderId = 1, // This links the OrderItem to the Order with Id 1
            ItemId = "P000001", // Example ItemId, adjust as needed
            Amount = 15 // Example quantity, adjust as needed
        });

        context.OrderItems.Add(new OrderItem
        {
            Id = 2,
            OrderId = 1, // This links the OrderItem to the Orde0 with Id 1
            ItemId = "P000002", // Example ItemId, adjust as needed
            Amount = 5 // Example quantity, adjust as needed
        });

        // Seed Shipments
        context.Shipments.Add(new Shipment
        {
            ShipmentId = 1,
            SourceId = 1,
            OrderId = "1",
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

        context.ShipmentItems.Add(new ShipmentItem
        {
            ShipmentId = 1,
            ItemId = "P000001",
            Amount = 15
        });

        context.ShipmentItems.Add(new ShipmentItem
        {
            ShipmentId = 1,
            ItemId = "P000002",
            Amount = 5
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
    public async Task TestUpdateShipmentStatus()
    {
        // Act
        string updatedStatus = await _shipmentService.UpdateShipmentStatusAsync(1, "In Transit"); // Correcte hoofdlettergevoelige status
        Console.WriteLine(updatedStatus);

        // Assert
        Assert.IsTrue(updatedStatus == $"Shipment {1} status successfully updated to 'In Transit'.");

        // Controleer wijzigingen in locaties en voorraad
        Location location1 = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == 1);
        Location location2 = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == 2);
        Inventory inventory1 = await _dbContext.Inventories.FirstOrDefaultAsync(l => l.InventoryId == 1);

        Console.WriteLine(location1.ItemAmounts["P000002"]);
        Console.WriteLine(location2.ItemAmounts["P000002"]);
        Console.WriteLine(location2.ItemAmounts["P000001"]);

        Assert.IsTrue(!location1.ItemAmounts.ContainsKey("P000001") && location2.ItemAmounts["P000001"] == 5);
        Assert.IsTrue(location1.ItemAmounts["P000002"] == 5 && location2.ItemAmounts["P000002"] == 10);
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

