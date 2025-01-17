using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;


namespace UnitTests
{
    [TestClass]
    public class FiltersUnitTests
    {
        private Filters _filters;
        private DefaultHttpContext _httpContext;

        [TestInitialize]
        public void Setup()
        {
            // Setup the configuration for API tokens
            var configurationDictionary = new Dictionary<string, string>
            {
                { "ApiKeys:AdminApiToken", "A1B2C3D4" },
                { "ApiKeys:EmployeeApiToken", "H8I9J10" },
                { "ApiKeys:FloorManagerApiToken", "E5F6G7" },
                { "ApiKeys:WarehouseManagerApiToken", "K11L12M13" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDictionary)
                .Build();

            // Instantiate the filter class
            _filters = new Filters(configuration);

            // Setup HttpContext for testing
            _httpContext = new DefaultHttpContext();
        }

        private void SetRequestHeaders(string apiToken, string method, string path)
        {
            _httpContext.Request.Headers["ApiToken"] = apiToken;
            _httpContext.Request.Method = method;
            _httpContext.Request.Path = path;
        }

        private ActionExecutingContext CreateActionContext()
        {
            var actionContext = new ActionContext { HttpContext = _httpContext };
            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), null);
        }

        [TestMethod]
        public async Task TestAdminTokenGetAccess()
        {
            SetRequestHeaders("A1B2C3D4", "GET", "/api/v1/Clients");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task TestAdminTokenPostAccess()
        {
            SetRequestHeaders("A1B2C3D4", "POST", "/api/v1/Clients");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task TestFloorManagerTokenPutAccessAllowedPath()
        {
            SetRequestHeaders("E5F6G7", "PUT", "/api/v1/Orders/1");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task TestFloorManagerTokenPutAccessDisallowedPath()
        {
            SetRequestHeaders("E5F6G7", "PUT", "/api/v1/Clients/1");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.AreEqual("Forbidden: Floor Manager is not authorized for this request.", await ReadResponseBodyAsync());
        }

        [TestMethod]
        public async Task TestWarehouseManagerTokenPutAccessAllowedPath()
        {
            SetRequestHeaders("K11L12M13", "PUT", "/api/v1/Warehouses/1");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task TestWarehouseManagerTokenPutAccessDisallowedPath()
        {
            SetRequestHeaders("K11L12M13", "PUT", "/api/v1/Clients/9830");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.AreEqual("Forbidden: Warehouse Manager is not authorized for this request.", await ReadResponseBodyAsync());
        }

        [TestMethod]
        public async Task TestInvalidToken()
        {
            SetRequestHeaders("InvalidToken", "GET", "/api/v1/Orders");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.AreEqual("Forbidden: Invalid or unauthorized ApiToken.", await ReadResponseBodyAsync());
        }

        [TestMethod]
        public async Task TestMissingToken()
        {
            SetRequestHeaders("", "GET", "/api/v1/Orders");

            var context = CreateActionContext();

            await _filters.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), null)));

            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.AreEqual("Unauthorized: Missing ApiToken header.", await ReadResponseBodyAsync());
        }

        private async Task<string> ReadResponseBodyAsync()
        {
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
