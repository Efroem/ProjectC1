using Microsoft.AspNetCore.Mvc.Filters;
public class Filters : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;
    private readonly string[] _floorManagerAllowedPaths;

    public Filters(IConfiguration configuration)
    {
        _configuration = configuration;
        _floorManagerAllowedPaths = new[] { "/api/v1/Orders", "/api/v1/Shipments" };
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var adminToken = _configuration["ApiKeys:AdminApiToken"];
        var employeeToken = _configuration["ApiKeys:EmployeeApiToken"];
        var floorManagerToken = _configuration["ApiKeys:FloorManagerApiToken"];

        if (string.IsNullOrEmpty(adminToken) || string.IsNullOrEmpty(employeeToken) || string.IsNullOrEmpty(floorManagerToken))
        {
            Console.WriteLine("One or more API tokens are not properly configured.");
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("API tokens are not properly configured.");
            return;
        }

        if (!httpContext.Request.Headers.ContainsKey("ApiToken"))
        {
            Console.WriteLine($"Request to {httpContext.Request.Path} is missing the ApiToken header.");
            httpContext.Response.StatusCode = 401; // Unauthorized
            await httpContext.Response.WriteAsync("Missing ApiToken header.");
            return;
        }

        var apiToken = httpContext.Request.Headers["ApiToken"];
        var requestPath = httpContext.Request.Path;

        if (apiToken == adminToken)
        {
            Console.WriteLine("Admin access granted.");
            await next(); 
        }
        else if (apiToken == employeeToken && HttpMethods.IsGet(httpContext.Request.Method))
        {
            Console.WriteLine("Employee access granted for GET request.");
            await next(); // Proceed for Employees (only GET requests)
        }
        else if (apiToken == floorManagerToken)
        {
            if (_floorManagerAllowedPaths.Any(path => requestPath.StartsWithSegments(path)))
            {
                Console.WriteLine($"Floor Manager access granted for path: {requestPath}.");
                await next(); // Proceed for Floor Managers within allowed paths
            }
            else
            {
                Console.WriteLine($"Access denied for Floor Manager on path: {requestPath}.");
                httpContext.Response.StatusCode = 403; // Forbidden
                await httpContext.Response.WriteAsync("Access denied. Floor Manager is not authorized for this path.");
            }
        }
        else
        {
            Console.WriteLine($"Access denied for invalid or unauthorized ApiToken: {apiToken}.");
            httpContext.Response.StatusCode = 403; // Forbidden
            await httpContext.Response.WriteAsync("Access denied.");
        }
    }
}
