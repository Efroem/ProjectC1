using CargoHubRefactor.Services;
using Microsoft.EntityFrameworkCore;

[TestClass]
public class UnitTest_Reporting
{
    private CargoHubDbContext _dbContext;
    private ReportingService _reportingService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        SeedDatabase(_dbContext);
        _reportingService = new ReportingService(_dbContext);
    }

    private void SeedDatabase(CargoHubDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Clients
        context.Clients.AddRange(new List<Client>
        {
            new Client { ClientId = 1, Name = "John Doe", Address = "123 Main St", City = "Springfield", ZipCode = "12345", Province = "Province1", Country = "Country1", ContactName = "John Contact", ContactPhone = "123-456-7890", ContactEmail = "john@example.com", CreatedAt = new DateTime(2023, 01, 01), UpdatedAt = DateTime.UtcNow },
            new Client { ClientId = 2, Name = "Jane Doe", Address = "456 Elm St", City = "Shelbyville", ZipCode = "67890", Province = "Province2", Country = "Country2", ContactName = "Jane Contact", ContactPhone = "098-765-4321", ContactEmail = "jane@example.com", CreatedAt = new DateTime(2023, 05, 01), UpdatedAt = DateTime.UtcNow }
        });

        // Seed Warehouses
        context.Warehouses.AddRange(new List<Warehouse>
        {
            new Warehouse { WarehouseId = 1, Name = "Central Warehouse", Address = "789 Storage Ln", City = "Storageville", Zip = "54321", Province = "Province3", Country = "Country3", Code = "CW001", ContactName = "Warehouse Contact", ContactPhone = "555-1234", ContactEmail = "contact@warehouse.com", CreatedAt = new DateTime(2023, 01, 01), UpdatedAt = DateTime.UtcNow }
        });

        context.SaveChanges();
    }

    [TestMethod]
    public void GenerateReport_ShouldReturnClientsData_WhenEntityIsClients()
    {
        var result = _reportingService.GenerateReport("clients", new DateTime(2023, 01, 01), new DateTime(2023, 12, 31), null);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void GenerateReport_ShouldThrowArgumentException_WhenEntityIsInvalid()
    {
        Assert.ThrowsException<ArgumentException>(() => _reportingService.GenerateReport("invalidEntity", DateTime.Now, DateTime.Now, null));
    }

    [TestMethod]
    public void GenerateReport_ShouldFail_WhenDateRangeIsInvalid()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _reportingService.GenerateReport(
                "clients",
                new DateTime(2024, 01, 01), // FromDate is later than ToDate
                new DateTime(2023, 12, 31),
                null
            )
        );
    }


    [TestMethod]
    public void GenerateReport_ShouldIncludeWarehouseId_WhenWarehouseEntityIsUsed()
    {
        var result = _reportingService.GenerateReport("warehouses", new DateTime(2023, 01, 01), new DateTime(2023, 12, 31), 1);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GenerateReport_ShouldFail_WhenNoFiltersAreProvided()
    {
        var result = _reportingService.GenerateReport("clients", default, default, null);
        Assert.AreEqual(0, result.Count());
    }
}
