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
            UpdatedAt = DateTime.UtcNow,
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

    [TestMethod]
    public void TestAddOrder()
    {
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
            orderItems: new List<OrderItem>{}
        );

        Assert.IsNotNull(newOrder.Result);
        Assert.AreEqual(2, _dbContext.Orders.Count());
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
                totalSurcharge: 10.00,
                orderItems: new List<OrderItem>{}
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
