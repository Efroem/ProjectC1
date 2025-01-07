namespace UnitTests
{
    using CargoHubRefactor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class UnitTest_Client
    {
        private CargoHubDbContext _dbContext;
        private ClientService _clientService;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CargoHubDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCargoHubDatabase")
                .Options;

            _dbContext = new CargoHubDbContext(options);
            _clientService = new ClientService(_dbContext);
            SeedDatabase(_dbContext);
        }

        private void SeedDatabase(CargoHubDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Clients.AddRange(
                new Client
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
                },
                new Client
                {
                    ClientId = 2,
                    Name = "Xander",
                    Address = "456 Street B",
                    City = "CityB",
                    ZipCode = "67890",
                    Province = "Kiev",
                    Country = "Ukraine",
                    ContactName = "Xandertje",
                    ContactPhone = "011231231",
                    ContactEmail = "xanderbos@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Client
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
                },
                new Client
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
                }
            );

            context.SaveChanges();
        }

        [TestMethod]
        public async Task TestGetAllClients()
        {
            var clients = await _clientService.GetClientsAsync();
            Assert.AreEqual(4, clients.Count());
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(99999, false)]
        public async Task TestGetClientById(int clientId, bool exists)
        {
            var client = await _clientService.GetClientAsync(clientId);
            Assert.AreEqual(exists, client != null);
        }

        [TestMethod]
        public async Task TestAddClient()
        {
            var newClient = await _clientService.AddClientAsync(
                "New Client",
                "789 Street C",
                "CityC",
                "33333",
                "ProvinceC",
                "CountryC",
                "Contact C",
                "1231231234",
                "contactc@example.com");

            Assert.IsNotNull(newClient);
            Assert.AreEqual(5, (await _clientService.GetClientsAsync()).Count());
        }

        [TestMethod]
        [DataRow("vincent@gmail.com", false)]
        [DataRow("newclient@example.com", true)]
        [DataRow("xanderbos@gmail.com", false)]
        [DataRow("uniqueemail123@example.com", true)]
        public async Task TestClientEmailDuplicateCheck(string email, bool expectedResult)
        {
            var clients = await _clientService.GetClientsAsync();
            var isEmailUnique = !clients.Any(c => c.ContactEmail == email);
            Assert.AreEqual(expectedResult, isEmailUnique);
        }

        [TestMethod]
        [DataRow(1, "Updated Name", true)]
        [DataRow(999, "Nonexistent", false)]
        public async Task TestUpdateClient(int clientId, string updatedName, bool expectedResult)
        {
            try
            {
                var updatedClient = await _clientService.UpdateClientAsync(
                    clientId,
                    updatedName,
                    "Updated Address",
                    "Updated City",
                    "00000",
                    "Updated Province",
                    "Updated Country",
                    "Updated Contact",
                    "0000000000",
                    "updated@example.com");

                Assert.IsNotNull(updatedClient);
                Assert.AreEqual(updatedName, updatedClient.Name);
            }
            catch (KeyNotFoundException)
            {
                Assert.IsFalse(expectedResult);
            }
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(999, false)]
        public async Task TestDeleteClient(int clientId, bool expectedResult)
        {
            var result = await _clientService.DeleteClientAsync(clientId);
            Assert.AreEqual(expectedResult, result);

            if (result)
            {
                var client = await _clientService.GetClientAsync(clientId);
                Assert.IsNull(client);
            }
        }
    }
}
