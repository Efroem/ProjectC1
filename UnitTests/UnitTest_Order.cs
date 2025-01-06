namespace UnitTests;
using CargoHubRefactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UnitTest_Order
{
    private CargoHubDbContext _dbContext;
    private OrderService orderService;
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
            .Options;

        _dbContext = new CargoHubDbContext(options);
        orderService = new OrderService(_dbContext);
        SeedDatabase(_dbContext);
    }

    private void SeedDatabase(CargoHubDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

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
            Price = 4.55,
            Weight = 6.42,
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
            ItemId = "ITEM-001", // Example ItemId, adjust as needed
            Amount = 10 // Example quantity, adjust as needed
        });

        context.OrderItems.Add(new OrderItem
        {
            Id = 2,
            OrderId = 1, // This links the OrderItem to the Order with Id 1
            ItemId = "ITEM-001", // Example ItemId, adjust as needed
            Amount = 15 // Example quantity, adjust as needed
        });


        context.SaveChanges();
    }

    [TestMethod]
    public void TestGetAllOrders()
    {
        var orders = orderService.GetOrdersAsync();
        Assert.AreEqual(1, orders.Result.Count());
    }

    [TestMethod]
    [DataRow(1, true)]
    [DataRow(99999, false)]
    public void TestGetOrderById(int orderId, bool exists)
    {
        var order = orderService.GetOrderAsync(orderId);
        Assert.AreEqual(exists, order.Result != null);
    }

    // [TestMethod]
    // public void 

    [TestMethod]
    [DataRow(0, true)]
    [DataRow(1, true)]
    [DataRow(2, false)]
  
    public void TestAddOrder(int OrderItemListId, bool expectedResult)
    {
        int orderItemAmountBefore;
        if (_dbContext.OrderItems.Any())
        {
            orderItemAmountBefore = _dbContext.OrderItems.Max(l => l.Id);
        }
        else
        {
            orderItemAmountBefore = 0;
        }
        List<List<OrderItem>> OrderItemLists = new List<List<OrderItem>>() {
            new List<OrderItem>() {},
            new List<OrderItem>{
                new OrderItem{
                    ItemId = "P000001",
                    Amount = 5
                },
                new OrderItem{
                    ItemId = "P000002",
                    Amount = 3
                }
            },
            new List<OrderItem>{
                new OrderItem{
                    ItemId = "P000001",
                    Amount = 5
                },
                new OrderItem{
                    ItemId = "Invalid",
                    Amount = 3
                }
            }
        };

        var newOrder = orderService.AddOrderAsync(
            sourceId: 2,
            orderDate: DateTime.UtcNow.AddDays(-7),
            requestDate: DateTime.UtcNow.AddDays(-3),
            reference: "ORD-12345 Test",
            referenceExtra: "Special handling required Test",
            orderStatus: "Processing Test",
            notes: "Customer prefers expedited shipping Ttest.",
            shippingNotes: "Fragile items. Handle with care. Test",
            pickingNotes: "Verify quantities before packing. Test",
            warehouseId: 5,
            shipTo: 2001,
            billTo: 2002,
            shipmentId: 3001,
            totalAmount: 500.75,
            totalDiscount: 50.00,
            totalTax: 25.50,
            totalSurcharge: 10.00,
            orderItems: OrderItemLists[OrderItemListId]
        ).Result;

        int orderItemAmountAfter;
        if (_dbContext.OrderItems.Any())
        {
            orderItemAmountAfter = _dbContext.OrderItems.Max(l => l.Id);
        }
        else
        {
            orderItemAmountAfter = 0;
        }

        Console.WriteLine($"Before: {orderItemAmountBefore}, After: {orderItemAmountAfter}");

        Assert.IsNotNull(newOrder);
        Assert.AreEqual(2, _dbContext.Orders.Count());
        switch (OrderItemListId) {
            case 0:
                break;
            case 1: 
                Assert.IsTrue(orderItemAmountBefore + 2 == orderItemAmountAfter);
                break;
            case 2:
                Assert.IsTrue(orderItemAmountBefore + 1 == orderItemAmountAfter);
                break;
        }
    }

    [TestMethod]
    [DataRow(1, "Fragile items. Handle with care. Updated" , true)]
    [DataRow(999, "Nonexistent", false)]
    public void TestUpdateOrder(int orderId, string updatedShipmentNotes, bool expectedResult)
    {
        try
        {
            var updatedOrder = orderService.UpdateOrderAsync(
                id: orderId, 
                sourceId: 2,
                orderDate: DateTime.UtcNow.AddDays(-7),
                requestDate: DateTime.UtcNow.AddDays(-3),
                reference: "ORD-12345 Updated",
                referenceExtra: "Special handling required Updated",
                orderStatus: "Processing Updated",
                notes: "Customer prefers expedited shipping Updated.",
                shippingNotes: updatedShipmentNotes,
                pickingNotes: "Verify quantities before packing. Updated",
                warehouseId: 5,
                shipTo: 2001,
                billTo: 2002,
                shipmentId: 3001,
                totalAmount: 500.75,
                totalDiscount: 50.00,
                totalTax: 25.50,
                totalSurcharge: 10.00
            );

            Assert.AreEqual(updatedOrder.Result != null, expectedResult);
            if (updatedOrder.Result != null) {
                Assert.AreEqual(updatedShipmentNotes == updatedOrder.Result.ShippingNotes, expectedResult);
            }
        }
        catch (KeyNotFoundException)
        {
            Assert.IsFalse(expectedResult);
        }
    }

    [TestMethod]
    [DataRow(1, true)]
    [DataRow(999, false)]
    public void TestDeleteOrder(int orderId, bool expectedResult)
    {
        var result = orderService.DeleteOrderAsync(orderId);
        Assert.AreEqual(expectedResult, result.Result);
        if (result.Result)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderId);
            Assert.IsNull(order);
        }
    }
}
