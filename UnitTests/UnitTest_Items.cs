namespace UnitTests;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
WHEN MAKING A NEW UNIT TEST FILE FOR AN ENDPOINT. COPY OVER Setup AND SeedDatabase
*/

[TestClass]
public class UnitTest_Item
{
    // ================ VV Copy this box when making a new Unit Test file VV ====================
    private CargoHubDbContext _dbContext;
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });


        context.Locations.Add(new Location
        {
            LocationId = 1, // Ensure unique LocationId
            WarehouseId = 1, // Reference the associated WarehouseId
            Code = "LOC001", // Unique code for the location
            Name = "Aisle 1",
            ItemAmounts = new Dictionary<string, int> // Populate the ItemAmounts dictionary
            {
                { "P000001", 10 }, // Example: Item P000001 with 10 units
                { "P000002", 5 }   // Example: Item P000002 with 5 units
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Item with a unique ItemId
        context.Inventories.Add(new Inventory { 
            InventoryId = 1,  // Ensure unique ItemId
            ItemId = "P000001",  // Reference the unique ItemId
            Description = "dummy",
            ItemReference = "dummy",
            LocationsList = [
                1,
                2,
                3
            ],
            TotalOnHand = 100,
            TotalExpected = 1,
            TotalOrdered = 1,
            TotalAllocated = 1,
            TotalAvailable = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        
        context.SaveChanges();
    }

    // ======================== ^^ Copy this when making a new Unit Test file ^^ =====================

    [TestMethod]
    public void TestGetAll()
    {
        ItemService ItemService = new ItemService(_dbContext);
        List<Item> ItemList = ItemService.GetItemsAsync().Result.ToList();
        Assert.IsTrue(ItemList.Count >= 1);
    }

    [TestMethod]
    [DataRow("P000001", true)]  
    [DataRow("P000999", false)] 
    public void TestGetById(string ItemId, Boolean expectedresult) {
        ItemService ItemService = new ItemService(_dbContext);
        Item? Item = ItemService.GetItemByIdAsync(ItemId).Result;
        Assert.AreEqual((Item != null), expectedresult);
    }

    [TestMethod]
    [DataRow("P000001", 1, true)]  
    [DataRow("P000001", 4, false)] 
    [DataRow("P000999", 1, false)] 

    public void TestGetAmountByLocationId(string ItemId, int LocationId, Boolean expectedresult) {
        ItemService ItemService = new ItemService(_dbContext);
        int? itemAmount = ItemService.GetItemAmountAtLocationByIdAsync(ItemId, LocationId).Result;
        Assert.AreEqual(itemAmount != -1, expectedresult);
    }

    [TestMethod]
    [DataRow("P000100", "dummyCode", true)]  
    [DataRow("P000015", null, false)]  
    public void TestPost(string ItemId, string commodityCode, Boolean expectedresult) {
        Item item = new Item {
            Uid = ItemId,  // Unique Item Uid
            Code = "DummydummyDummyDumm",
            Description = "dummyDummydummyDummyDumm",
            ShortDescription = "dummyDummdummyDummyDummy",
            UpcCode = "dummyDummydummyDummyDumm",
            ModelNumber = "dummyDdummyDummyDummummy",
            CommodityCode = commodityCode,
            ItemLine = 1,  // Reference the unique ItemLine ID
            ItemGroup = 1,  // Reference the unique ItemGroup ID
            ItemType = 1,  // Reference the unique ItemType ID
            UnitPurchaseQuantity = 1,
            UnitOrderQuantity = 1,
            PackOrderQuantity = 1,
            SupplierId = 2,
            SupplierCode = "dummyDdummyDummyDummummy",
            SupplierPartNumber = "dummyDdummyDummyDummummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ItemService ItemService = new ItemService(_dbContext);
        (string message, Item? returnedItem) result = ItemService.AddItemAsync(item).Result;
        Assert.AreEqual(result.returnedItem != null, expectedresult);
        // TestContext.WriteLine(result.message);
        // Assert.IsTrue(result.returnedItem != null);
    }


    [TestMethod]
    [DataRow("P000001", "UpdCommodityCode", true)]  
    [DataRow("P000001", null, false)]  
    public void TestPut(string ItemId, string commodityCode, Boolean expectedresult) {
        Item item = new Item {
            Uid = ItemId,  // Unique Item Uid
            Code = "dummyDummyDummy",
            Description = "dummyDummyDumm",
            ShortDescription = "dummyDummyDumm",
            UpcCode = "dummyDummyDumm",
            ModelNumber = "dummyDummyDumm",
            CommodityCode = commodityCode,
            ItemLine = 1,  // Reference the unique ItemLine ID
            ItemGroup = 1,  // Reference the unique ItemGroup ID
            ItemType = 1,  // Reference the unique ItemType ID
            UnitPurchaseQuantity = 1,
            UnitOrderQuantity = 1,
            PackOrderQuantity = 1,
            SupplierId = 2,
            SupplierCode = "dummyDummyDumm",
            SupplierPartNumber = "dummyDummyDummy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ItemService ItemService = new ItemService(_dbContext);
        Item? returnedItem = ItemService.UpdateItemAsync(ItemId, item).Result.returnedItem;
        Assert.AreEqual(returnedItem != null, expectedresult);
    }


    [TestMethod]
    [DataRow("P000001", true)]
    [DataRow("P999999", false)]
    public void TestDelete (string ItemId, bool expectedresult) {
        ItemService ItemService = new ItemService(_dbContext);
        bool succesfullyDeleted = ItemService.DeleteItemAsync(ItemId).Result;
        Assert.AreEqual(succesfullyDeleted, expectedresult);
    }
}