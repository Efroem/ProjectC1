namespace UnitTests;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
WHEN MAKING A NEW UNIT TEST FILE FOR AN ENDPOINT. COPY OVER Setup AND SeedDatabase
*/

[TestClass]
public class UnitTest_Supplier
{
    // ================ VV Copy this box when making a new Unit Test file VV ====================
    private CargoHubDbContext _dbContext;
    private SupplierService supplierService;
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
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    // Seed Suppliers with unique IDs and complete data
    context.Suppliers.Add(new Supplier
    {
        SupplierId = 1,
        Code = "SUP001",
        Name = "Supplier A",
        Address = "123 Main Street",
        AddressExtra = "Building A",  // Explicitly set the extra address information
        City = "Metropolis",
        ZipCode = "12345",
        Province = "Central",  // Explicitly set the province
        Country = "Wonderland",
        ContactName = "John Doe",
        PhoneNumber = "123-456-7890",
        Reference = "REF123",  // Explicitly set the reference
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    });

    context.Suppliers.Add(new Supplier
    {
        SupplierId = 2,
        Code = "SUP002",
        Name = "Supplier B",
        Address = "456 Side Street",
        AddressExtra = "Building B",
        City = "Gotham",
        ZipCode = "67890",
        Province = "Central",
        Country = "Neverland",
        ContactName = "Jane Smith",
        PhoneNumber = "098-765-4321",
        Reference = "REF123",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    });

    context.SaveChanges();
}


    // ======================== ^^ Copy this when making a new Unit Test file ^^ =====================

    [TestMethod]
    public void TestGetAllSuppliers()
    {
        SupplierService supplierService = new SupplierService(_dbContext);
        List<Supplier> supplierList = supplierService.GetAllSuppliersAsync().Result.ToList();
        Assert.IsTrue(supplierList.Count >= 2);
    }


    [TestMethod]
    [DataRow(1, true)]
    [DataRow(999, false)]
    public void TestGetSupplierById(int supplierId, bool expectedresult)
    {
        SupplierService supplierService = new SupplierService(_dbContext);
        Supplier? supplier = supplierService.GetSupplierByIdAsync(supplierId).Result;
        Assert.AreEqual(supplier != null, expectedresult);
    }

    [TestMethod]
    [DataRow(3, "SUP003", "New Supplier", "123 Example Street", "Example City", "EX123", "Exampleland", "Alice Johnson", "123-123-1234", true)]
    [DataRow(4, null, "Invalid Supplier", "No Address", "No City", "NOZIP", "Noland", "Invalid Contact", "987-654-3210", false)]
    public void TestPostSupplier(int supplierId, string code, string name, string address, string city, string zipCode, string country, string contactName, string phoneNumber, bool expectedresult)
    {
        Supplier supplier = new Supplier
        {
            SupplierId = supplierId,
            Code = code,
            Name = name,
            Address = address,
            City = city,
            ZipCode = zipCode,
            Country = country,
            ContactName = contactName,
            PhoneNumber = phoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Reference = ""
        };

        SupplierService supplierService = new SupplierService(_dbContext);
        Supplier? returnedSupplier = supplierService.CreateSupplierAsync(supplier).Result;
        Assert.AreEqual(returnedSupplier != null, expectedresult);
    }


    [TestMethod]
    [DataRow(1, "Updated Supplier", "SUP001-NEW", "789 Update Ave", "Updated City", "UP123", "Updateland", "Updated Contact", "456-456-4567", true)]
    [DataRow(999, null, null, null, null, null, null, null, null, false)]
    public void TestPutSupplier(int supplierId, string name, string code, string address, string city, string zipCode, string country, string contactName, string phoneNumber, bool expectedresult)
    {
        Supplier supplier = new Supplier
        {
            SupplierId = supplierId,
            Code = code,
            Name = name,
            Address = address,
            City = city,
            ZipCode = zipCode,
            Country = country,
            ContactName = contactName,
            PhoneNumber = phoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        SupplierService supplierService = new SupplierService(_dbContext);
        bool updated = supplierService.UpdateSupplierAsync(supplierId, supplier).Result;
        Assert.AreEqual(updated, expectedresult);
    }


    [TestMethod]
    [DataRow(1, true)]
    [DataRow(999, false)]
    public void TestDeleteSupplier(int supplierId, bool expectedresult)
    {
        SupplierService supplierService = new SupplierService(_dbContext);
        bool deleted = supplierService.DeleteSupplierAsync(supplierId).Result;
        Assert.AreEqual(deleted, expectedresult);
    }

    [TestMethod]
    public void TestDeleteAllSuppliers()
    {
        SupplierService supplierService = new SupplierService(_dbContext);
        bool deleted = supplierService.DeleteAllSuppliersAsync().Result;
        Assert.IsTrue(deleted);
    }
}
