namespace UnitTests;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
WHEN MAKING A NEW UNIT TEST FILE FOR AN ENDPOINT. COPY OVER Setup AND SeedDatabase
*/

[TestClass]
public class UnitTest_Inventory
{
    // ================ VV Copy this box when making a new Unit Test file VV ====================
    private CargoHubDbContext _dbContext;
    private InventoryService inventoryService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        // Ensure Microsoft.EntityFrameworkCore and Microsoft.EntityFrameworkCore.InMemory are installed
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        // Seed test data
        SeedDatabase(_dbContext);

    }

    private void SeedDatabase(CargoHubDbContext context)
    {
        // Clear existing data to avoid conflicts
        context.Database.EnsureDeleted();  // Ensure database is cleared before seeding
        context.Database.EnsureCreated();  // Create the database if not already created

        // Seed ItemGroups with unique IDs
        context.ItemGroups.Add(new ItemGroup {
            GroupId = 1,  // Ensure unique GroupId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemGroups.Add(new ItemGroup {
            GroupId = 2,  // Ensure unique GroupId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed ItemTypes with unique IDs
        context.ItemTypes.Add(new ItemType {
            TypeId = 1,  // Ensure unique TypeId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemTypes.Add(new ItemType {
            TypeId = 2,  // Ensure unique TypeId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed ItemLines with unique IDs
        context.ItemLines.Add(new ItemLine {
            LineId = 1,  // Ensure unique LineId
            Name = "dummy",
            Description = "Dummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.ItemLines.Add(new ItemLine {
            LineId = 2,  // Ensure unique LineId
            Name = "dummy2",
            Description = "Dummy2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Items with unique codes and references
        context.Items.Add(new Item {
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

        context.Items.Add(new Item {
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
        context.Items.Add(new Item {
            Uid = "P000011",  // Unique Item Uid
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
            Classification = "DummyRestricted",
            SupplierId = 1,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Items.Add(new Item {
            Uid = "P000012",  // Unique Item Uid
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
            Height = 10000,
            Width = 10,
            Depth = 10,
            SupplierId = 2,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Items.Add(new Item {
            Uid = "P000013",  // Unique Item Uid
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
            Height = 10,
            Width = 10000,
            Depth = 10,
            SupplierId = 1,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Items.Add(new Item {
            Uid = "P000014",  // Unique Item Uid
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
            Height = 10,
            Width = 100000,
            Depth = 10,
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
            RestrictedClassificationsList = new List<string>{},
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        
        context.Warehouses.Add(new Warehouse
        {
            WarehouseId = 2, // Ensure unique WarehouseId
            Code = "WH002",  // Unique code for the warehouse
            Name = "Main Warehouse",
            Address = "123 Warehouse St.",
            Zip = "12345",
            City = "Sample City",
            Province = "Sample Province",
            Country = "Sample Country",
            ContactName = "John Doe",
            ContactPhone = "555-1234",
            ContactEmail = "johndoe@example.com",
            RestrictedClassificationsList = new List<string>{"DummyRestricted"},
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Inventory with a unique InventoryId
        context.Inventories.Add(new Inventory { 
            InventoryId = 1,  // Ensure unique InventoryId
            ItemId = "P000001",  // Reference the unique ItemId
            Description = "dummy",
            ItemReference = "dummy",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Locations.Add(new Location
        {
            LocationId = 1,
            Name = "Row: A, Rack: 1, Shelf: 1",
            Code = "LOC001",
            WarehouseId = 1,
            ItemAmounts = new Dictionary<string, int>{},
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Locations.Add(new Location
        {
            LocationId = 2,
            Name = "Row: B, Rack: 2, Shelf: 2",
            Code = "LOC002",
            ItemAmounts = new Dictionary<string, int>{},
            WarehouseId = 2,
            MaxHeight = 20,
            MaxWidth = 20,
            MaxDepth = 20,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        
        context.SaveChanges();
    }

    // ======================== ^^ Copy this when making a new Unit Test file ^^ =====================

    [TestMethod]
    public void TestGetAll()
    {
        InventoryService inventoryService = new InventoryService(_dbContext);
        List<Inventory> inventoryList = inventoryService.GetInventoriesAsync().Result;
        Assert.IsTrue(inventoryList.Count >= 1);
    }

    [TestMethod]
    [DataRow(1, true)]  
    [DataRow(999, false)] 
    public void TestGetById(int inventoryId, Boolean expectedresult) {
        InventoryService inventoryService = new InventoryService(_dbContext);
        Inventory? inventory = inventoryService.GetInventoryByIdAsync(inventoryId).Result;
        Assert.AreEqual((inventory != null), expectedresult);
    }

    [TestMethod]
    [DataRow(2, "dummy", true)]  
    [DataRow(4, null, false)]  
    public void TestPost(int inventoryId, string description, Boolean expectedresult) {
        Inventory inventory = new Inventory { 
            InventoryId = inventoryId,  // Ensure unique InventoryId
            ItemId = "P000001",  // Reference the unique ItemId
            Description = description,
            ItemReference = "dummy",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            LocationsList = [
                1
            ],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        InventoryService inventoryService = new InventoryService(_dbContext);
        Inventory? returnedInventory = inventoryService.AddInventoryAsync(inventory).Result.returnedInventory;
        Assert.AreEqual(returnedInventory != null, expectedresult);
    }

    [TestMethod]
     public void TestPostInvalidItemId() {
        Inventory inventory = new Inventory { 
            InventoryId = 100009,  // Ensure unique InventoryId
            ItemId = "P999999",  // Reference the unique ItemId
            Description = "dummyDescription",
            ItemReference = "dummy1143q535",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            LocationsList = new List<int> {
                1,
                2
            },
            TotalAvailable = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        InventoryService inventoryService = new InventoryService(_dbContext);
        Inventory? returnedInventory = inventoryService.AddInventoryAsync(inventory).Result.returnedInventory;
        Assert.AreEqual(returnedInventory != null, false);
    }

    [TestMethod]
    public async Task TestPostInvalidItemClassification()
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 100009,  // Ensure unique InventoryId
            ItemId = "P000011",    // Reference the unique ItemId
            Description = "dummyDescription",
            ItemReference = "dummy1143q535",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            LocationsList = new List<int> { 1, 2 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Use a mocked or properly configured DbContext
        var inventoryService = new InventoryService(_dbContext);

        // Act
        var result = await inventoryService.AddInventoryAsync(inventory);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.returnedInventory);
        Assert.AreEqual(result.returnedInventory.LocationsList.Count, 1);
    }

    [TestMethod]
    public async Task TestPostInvalidItemHeight()
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 100009,  // Ensure unique InventoryId
            ItemId = "P000012",    // Reference the unique ItemId
            Description = "dummyDescription",
            ItemReference = "dummy1143q535",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            LocationsList = new List<int> { 1, 2 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Use a mocked or properly configured DbContext
        var inventoryService = new InventoryService(_dbContext);

        // Act
        var result = await inventoryService.AddInventoryAsync(inventory);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.returnedInventory);
        Assert.AreEqual(result.returnedInventory.LocationsList.Count, 1);
    }

        [TestMethod]
    public async Task TestPostInvalidItemWidth()
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 100009,  // Ensure unique InventoryId
            ItemId = "P000013",    // Reference the unique ItemId
            Description = "dummyDescription",
            ItemReference = "dummy1143q535",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            LocationsList = new List<int> { 1, 2 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Use a mocked or properly configured DbContext
        var inventoryService = new InventoryService(_dbContext);

        // Act
        var result = await inventoryService.AddInventoryAsync(inventory);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.returnedInventory);
        Assert.AreEqual(result.returnedInventory.LocationsList.Count, 1);
    }

    [TestMethod]
    public async Task TestPostInvalidItemDepth()
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 100009,  // Ensure unique InventoryId
            ItemId = "P000014",    // Reference the unique ItemId
            Description = "dummyDescription",
            ItemReference = "dummy1143q535",
            TotalOnHand = 1,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            LocationsList = new List<int> { 1, 2 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Use a mocked or properly configured DbContext
        var inventoryService = new InventoryService(_dbContext);

        // Act
        var result = await inventoryService.AddInventoryAsync(inventory);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.returnedInventory);
        Assert.AreEqual(result.returnedInventory.LocationsList.Count, 1);
    }

    [TestMethod]
    [DataRow(1, "UpdatedReference", true)]  
    [DataRow(1, null, false)]  
    public void TestPut(int inventoryId, string itemReference, Boolean expectedresult) {
        Inventory inventory = new Inventory { 
            InventoryId = inventoryId,  // Ensure unique InventoryId
            ItemId = "P000001",  // Reference the unique ItemId
            Description = "dummyUpdated",
            ItemReference = itemReference,
            TotalOnHand = 0,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        InventoryService inventoryService = new InventoryService(_dbContext);
        Inventory? returnedInventory = inventoryService.UpdateInventoryAsync(inventoryId, inventory).Result.returnedInventory;
        Assert.AreEqual(returnedInventory != null, expectedresult);
    }


    [TestMethod]
    [DataRow(1, true)]
    [DataRow(999999, false)]
    public void TestDelete (int inventoryId, bool expectedresult) {
        InventoryService inventoryService = new InventoryService(_dbContext);
        bool succesfullyDeleted = inventoryService.DeleteInventoryAsync(inventoryId).Result;
        Assert.AreEqual(succesfullyDeleted, expectedresult);
    }
}