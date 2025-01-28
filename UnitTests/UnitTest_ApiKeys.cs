namespace UnitTests;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

[TestClass]
public class UnitTest_APIKeys
{
    private CargoHubDbContext _dbContext;
    private ApiKeyService apiKeyService;
    private string envFilePath;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {

        // string projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "CargoHubRefactor"); // Adjust if needed
        // string fullPath = Path.GetFullPath(projectRoot);
        // envFilePath = Path.Combine(fullPath, ".env");

        // Console.WriteLine($"Loading .env file from: {envFilePath}");

        // // Ensure the .env file is loaded correctly
        // if (File.Exists(envFilePath))
        // {
        //     DotNetEnv.Env.Load(envFilePath);
        // }
        // else
        // {
        //     Console.WriteLine(".env file not found at expected path.");
        // }

        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        apiKeyService = new ApiKeyService(_dbContext);
        SeedDatabase(_dbContext);
    }

    private void SeedDatabase(CargoHubDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SHA256 mySha565 = SHA256.Create();

        // Seed ItemGroups with unique IDs
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
            Price = 4.55,
            Weight = 6.42,
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
            Price = 5.25,
            Weight = 3.42,
            SupplierId = 2,
            SupplierCode = "null",
            SupplierPartNumber = "null",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Clients.Add(new Client
        {
            ClientId = 1,
            Name = "Vincent",
            Address = "123 Street A",
            City = "Oosterland",
            ZipCode = "12345",
            Province = "Zeeland",
            Country = "Netherlands",
            ContactName = "Contact A",
            ContactPhone = "1234567890",
            ContactEmail = "vincent@gmail.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        context.Clients.Add(new Client
        {
            ClientId = 2,
            Name = "Xander",
            Address = "456 Street B",
            City = "CityB",
            ZipCode = "67890",
            Province = "Kiev",
            Country = "Oekräine",
            ContactName = "Xandertje",
            ContactPhone = "011231231",
            ContactEmail = "xanderbos@gmail.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Clients.Add(new Client
        {
            ClientId = 3,
            Name = "Nolan",
            Address = "456 Street B",
            City = "CityB",
            ZipCode = "67890",
            Province = "Groningen",
            Country = "Netherlands",
            ContactName = "Contact B",
            ContactPhone = "012312312",
            ContactEmail = "nolananimations@gmail.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Clients.Add(new Client
        {
            ClientId = 4,
            Name = "Efräim",
            Address = "456 Street B",
            City = "Ridderkerk",
            ZipCode = "67890",
            Province = "Zuid-Holland",
            Country = "Netherlands",
            ContactName = "Efräimpie",
            ContactPhone = "031231231",
            ContactEmail = "efräimcreampie@gmail.com",
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
            WarehouseId = 1,
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
            Amount = 10 // Example quantity, adjust as needed
        });

        context.OrderItems.Add(new OrderItem
        {
            Id = 2,
            OrderId = 1, // This links the OrderItem to the Order with Id 1
            ItemId = "P000002", // Example ItemId, adjust as needed
            Amount = 15 // Example quantity, adjust as needed
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

        context.APIKeys.Add(new APIKey
        {
            APIKeyId = 1,
            Name = "AdminApiToken",
            Key = "v�W���ƨ�B����v$7rԄtP��D-���"
        });

        context.APIKeys.Add(new APIKey
        {
            APIKeyId = 2,
            Name = "FloorManagerApiToken",
            Key = "P�t[���4 Ah8~n�w3}T�z�;c�"
        });

        context.APIKeys.Add(new APIKey
        {
            APIKeyId = 3,
            Name = "EmployeeApiToken",
            Key = "��}���j�����-@�|�vS3h=�v�tT�"
        });

        context.APIKeys.Add(new APIKey
        {
            APIKeyId = 4,
            Name = "WarehouseManagerToken",
            Key = "*ɨ�@A�K�0���iRU�;F ���ۇ�"
        });

        context.APIKeys.Add(new APIKey
        {
            APIKeyId = 5,
            Name = "EnvTestToken",
            Key = "WrongEnvKey"
        });


        context.SaveChanges();
    }

    [TestMethod]
    [DataRow("AdminApiToken")]
    [DataRow("FloorManagerApiToken")]
    [DataRow("EmployeeApiToken")]
    [DataRow("WarehouseManagerToken")]

    public async Task TestGetKey(string TokenName)
    {

        Console.WriteLine(TokenName);
        string apiKey = null;
        switch (TokenName) {
            case "AdminApiToken": 
                apiKey = await apiKeyService.GetAdminApiTokenAsync();
                break;

            case "FloorManagerApiToken": 
                apiKey = await apiKeyService.GetFloorManagerApiTokenAsync();
                break;

            case "EmployeeApiToken": 
                apiKey = await apiKeyService.GetEmployeeApiTokenAsync();
                break;
            case "WarehouseManagerToken": 
                apiKey = await apiKeyService.GetWarehouseManagerTokenAsync();
                break;
            default :
                apiKey = null;
                break;
        }
        Assert.IsTrue(apiKey != null);

        var compareKey = await _dbContext.APIKeys.FirstOrDefaultAsync(x => x.Name == TokenName);
        apiKey = apiKey.Replace("\0", ""); // Remove null characters

        Assert.IsTrue(compareKey != null);
        compareKey.Key = compareKey.Key.Replace("\0", ""); // Remove null characters
       


        Assert.AreEqual(apiKey.ToLowerInvariant().Trim(), compareKey.Key.ToLowerInvariant().Trim());

    }

    [TestMethod]
    public async Task TestGetKeyFromWorkflowEnvAsync() {
        string apiKey = await apiKeyService.GetEnvTestTokenAsync();
        Assert.IsTrue(apiKey != null);

        var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey); // Force UTF-8 encoding
        apiKey = Encoding.UTF8.GetString(apiKeyBytes);
        
        Assert.AreEqual(HashString("RightEnvKey").Trim(), apiKey.Trim());

    }

    public static string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return Encoding.Default.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(input)));
        }
    }
}