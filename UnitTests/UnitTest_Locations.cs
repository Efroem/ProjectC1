using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace UnitTests
{
    [TestClass]
    public class UnitTest_Location
    {
        private CargoHubDbContext _dbContext;
        private LocationService _locationService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CargoHubDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLocationDatabase")
                .Options;

            _dbContext = new CargoHubDbContext(options);
            SeedDatabase(_dbContext);
            _locationService = new LocationService(_dbContext);
        }

        private void SeedDatabase(CargoHubDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

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
                RestrictedClassificationsList = new List<string>{"DummyRestricted"},
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
                ItemAmounts = new Dictionary<string, int>{},
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            context.Inventories.Add(new Inventory { 
                InventoryId = 1,  // Ensure unique InventoryId
                ItemId = "P000001",  // Reference the unique ItemId
                Description = "dummy",
                ItemReference = "dummy",
                TotalOnHand = 100,
                TotalExpected = 1,
                TotalOrdered = 1,
                TotalAllocated = 1,
                TotalAvailable = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            context.Inventories.Add(new Inventory { 
                InventoryId = 2,  // Ensure unique InventoryId
                ItemId = "P000002",  // Reference the unique ItemId
                Description = "dummy2",
                ItemReference = "dummy2",
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


        [TestMethod]
        public async Task TestGetLocationById()
        {
            var location = await _locationService.GetLocationAsync(1);
            Assert.IsNotNull(location);
            Assert.AreEqual("Row: A, Rack: 1, Shelf: 1", location.Name);
        }

        [TestMethod]
        public async Task TestGetLocationById_NotFound()
        {
            var location = await _locationService.GetLocationAsync(999);
            Assert.IsNull(location);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(999999, false)]

        public async Task TestGetLocationByWarehouse(int id, bool expectedResult)
        {
            var locations = await _locationService.GetLocationsByWarehouseAsync(id);
            Assert.AreEqual(locations.Count() > 0, expectedResult);
        }
        
        [TestMethod]
        public async Task TestGetAllLocations()
        {
            var locations = await _locationService.GetLocationsAsync();
            Assert.AreEqual(3, locations.Count());
        }


        [TestMethod]
        [DataRow("Row: Z, Rack: 150, Shelf: 5", false)]  // Ongeldig rack (150 > 100)
        [DataRow("Row: A, Rack: 5, Shelf: 11", false)]    // Ongeldig shelf (11 > 10)
        [DataRow("Row: AA, Rack: 5, Shelf: 5", false)]    // Ongeldige naam (Row is geen enkele letter)
        [DataRow("Row: 1, Rack: 5, Shelf: 5", false)]    // Ongeldige naam (Row is geen letter)        
        [DataRow("Row: a, Rack: 5, Shelf: 5", false)]    // Ongeldige naam (Row is geen uppercase letter) 
        [DataRow("Row: A, Rack: 5, Shelf: 5", true)]      // Geldige invoer
        public async Task TestLocationNameValidation(string name, bool expectedResult)
        {
            var isValid = await _locationService.IsValidLocationNameAsync(name);
            Assert.AreEqual(expectedResult, isValid);
        }

        [TestMethod]
        [DataRow(100, 1, true)]
        [DataRow(100, 100, false)]
        [DataRow(1, 1, false)]
        public async Task TestAddLocation(int id, int warehouseId, bool expectedResult) {
            Location testLocation = new Location
            {
                LocationId = id,
                Name = "Row: G, Rack: 5, Shelf: 8",
                Code = "LOC100",
                ItemAmounts = new Dictionary<string, int>{},
                WarehouseId = warehouseId,
                MaxHeight = 20,
                MaxWidth = 20,
                MaxDepth = 20,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var returnedLocation = await _locationService.AddLocationAsync(testLocation);
        }

        [TestMethod]
        public async Task TestUpdateLocation_ValidData()
        {
            Location location = await _dbContext.Locations.FirstOrDefaultAsync(x => x.LocationId == 1);
            location.Name = "Row: D, Rack: 4, Shelf: 4";
            location.Code = "LOC001-Updated";
            location.MaxHeight = 20;
            location.MaxWidth = 20;
            location.MaxDepth = 20;
            location.MaxWeight = 100;
            var updatedLocation = await _locationService.UpdateLocationAsync(1, location);
            Assert.IsNotNull(updatedLocation);
            Assert.AreEqual("Row: D, Rack: 4, Shelf: 4", updatedLocation.Name);
        }

        [TestMethod]
        public async Task TestUpdateLocationItems_ValidData()
        {
            int id = 1;
            List<LocationItem> locationItems = new List<LocationItem>
            {
                new LocationItem { ItemId = "P000001", Amount = 15, Classification = "None", Height = 10, Depth = 10, Width = 10 },
                new LocationItem { ItemId = "P000002", Amount = 15, Classification = "None", Height = 10, Depth = 10, Width = 10 }
            };
            var updatedLocation = await _locationService.UpdateLocationItemsAsync(id, locationItems);
            Assert.IsTrue(updatedLocation !=  null);
            if (updatedLocation != null) {
                foreach (LocationItem item in locationItems) {
                    Assert.IsTrue(updatedLocation.ItemAmounts.ContainsKey(item.ItemId) && updatedLocation.ItemAmounts[item.ItemId] == item.Amount);
                }
            }

        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(999999, false)]
        public async Task TestUpdateLocationItems_InValidItemOrLocationId(int id, bool expectedResult)
        {
            List<LocationItem> locationItems = new List<LocationItem>
            {
                new LocationItem { ItemId = "P000001", Amount = 15, Classification = "None", Height = 10, Depth = 10, Width = 10 },
                new LocationItem { ItemId = "P999999", Amount = 15, Classification = "None", Height = 10, Depth = 10, Width = 10 }
            };
            var updatedLocation = await _locationService.UpdateLocationItemsAsync(id, locationItems);
            Assert.AreEqual(updatedLocation != null, expectedResult);
            if (updatedLocation != null) {
                Assert.IsTrue(updatedLocation.ItemAmounts.Count == 1);
            }

            
        }

        [TestMethod]
        [DataRow(10000, 10, 10)]
        [DataRow(10, 10000, 10)]
        [DataRow(10, 10, 10000)]
        public async Task TestUpdateLocationItems_TooHighSize(int height, int depth, int width)
        {
            int id = 2;
            List<LocationItem> locationItems = new List<LocationItem>
            {
                new LocationItem { ItemId = "P000002", Amount = 15, Classification = "None", Height = 10, Depth = 10, Width = 10 },
                new LocationItem { ItemId = "P000002", Amount = 15, Classification = "None", Height = height, Depth = depth, Width = width }
            };
            var updatedLocation = await _locationService.UpdateLocationItemsAsync(id, locationItems);
            Assert.IsTrue(updatedLocation !=  null);
            Console.WriteLine($"TooHighSize MaxHeight: {updatedLocation.MaxHeight}");
            Console.WriteLine($"TooHighSize ItemCount: {updatedLocation.ItemAmounts.Count}");
            Assert.IsTrue(updatedLocation.ItemAmounts.Count == 1);
        
        }

        [TestMethod]
        public async Task TestUpdateLocationItems_RestrictedCategory()
        {
            int id = 1;
            List<LocationItem> locationItems = new List<LocationItem>
            {
                new LocationItem { ItemId = "P000001", Amount = 15, Classification = "DummyRestricted", Height = 10, Depth = 10, Width = 10 }
            };
            var updatedLocation = await _locationService.UpdateLocationItemsAsync(id, locationItems);
            Assert.IsTrue(updatedLocation !=  null);
            Assert.IsTrue(updatedLocation.ItemAmounts.Count == 0);
        
        }

        [TestMethod]
        public async Task TestUpdateLocation_NotFound()
        {
            Location location = await _dbContext.Locations.FirstOrDefaultAsync(x => x.LocationId == 1);
            location.Name = "Row: E, Rack: 5, Shelf: 5";
            location.Code = "LOC999";
            location.WarehouseId = 4;
            var updatedLocation = await _locationService.UpdateLocationAsync(999, location);
            Assert.IsNull(updatedLocation);
        }

        [TestMethod]
        public async Task TestDeleteLocation_ValidId()
        {
            var result = await _locationService.DeleteLocationAsync(1);
            Assert.IsTrue(result);

            var location = await _locationService.GetLocationAsync(1);
            Assert.IsNull(location);
        }

        [TestMethod]
        public async Task TestDeleteLocation_InvalidId()
        {
            var result = await _locationService.DeleteLocationAsync(999);
            Assert.IsFalse(result);
        }
    }
}
