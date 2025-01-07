using Microsoft.AspNetCore.Mvc.Filters;

public class Filters : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;
    private readonly string[] _floorManagerAllowedPaths;
    private readonly string[] _warehouseManagerAllowedPaths;

    public Filters(IConfiguration configuration)
    {
        _configuration = configuration;
        _floorManagerAllowedPaths = new[] { "/api/v1/Orders", "/api/v1/Shipments" };
        _warehouseManagerAllowedPaths = new[] { "/api/v1/Warehouses" }; 
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var adminToken = _configuration["ApiKeys:AdminApiToken"];
        var employeeToken = _configuration["ApiKeys:EmployeeApiToken"];
        var floorManagerToken = _configuration["ApiKeys:FloorManagerApiToken"];
        var warehouseManagerToken = _configuration["ApiKeys:WarehouseManagerToken"];

        if (string.IsNullOrEmpty(adminToken) || string.IsNullOrEmpty(employeeToken) ||
            string.IsNullOrEmpty(floorManagerToken) || string.IsNullOrEmpty(warehouseManagerToken))
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("API tokens are not properly configured.");
            return;
        }

        if (!httpContext.Request.Headers.ContainsKey("ApiToken"))
        {
            httpContext.Response.StatusCode = 401;
            await httpContext.Response.WriteAsync("Missing ApiToken header.");
            return;
        }

        var apiToken = httpContext.Request.Headers["ApiToken"].ToString();
        var requestPath = httpContext.Request.Path;

        if (apiToken == adminToken)
        {
            await next();
            return;
        }

        if (apiToken == employeeToken && HttpMethods.IsGet(httpContext.Request.Method))
        {
            await next();
            return;
        }

        if (apiToken == floorManagerToken && HttpMethods.IsPut(httpContext.Request.Method))
        {
            if (_floorManagerAllowedPaths.Any(path => requestPath.StartsWithSegments(path)))
            {
                await next();
                return;
            }
            else
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync($"Floor Manager is not authorized for the path {requestPath}.");
                return;
            }
        }

        if (apiToken == warehouseManagerToken && HttpMethods.IsPut(httpContext.Request.Method))
        {
            if (_warehouseManagerAllowedPaths.Any(path => requestPath.StartsWithSegments(path)))
            {
                await next();
                return;
            }
            else
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync($"Warehouse Manager is not authorized for the path {requestPath}.");
                return;
            }
        }
        httpContext.Response.StatusCode = 403;
        await httpContext.Response.WriteAsync("Forbidden: Invalid or unauthorized ApiToken.");
    }
}
