using Microsoft.AspNetCore.Mvc.Filters;

public class FloorManagerFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;
    private readonly string[] _allowedPaths;

    public FloorManagerFilter(IConfiguration configuration)
    {
        _configuration = configuration;
        _allowedPaths = new[] { "/api/v1/Orders", "/api/v1/Shipments" };
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
    {
        var context = actionContext.HttpContext;

        var expectedToken = _configuration["ApiKeys:FloorManagerApiToken"];

        if (string.IsNullOrEmpty(expectedToken))
        {
            Console.WriteLine("Check om te kijken of de floormanagertoken juist in appsettings staat (error)");
            context.Response.StatusCode = 500; 
            await context.Response.WriteAsync("API token is not properly configured.");
            return;
        }

        if (!context.Request.Headers.ContainsKey("ApiToken"))
        {
            Console.WriteLine($"{context.Request.Path} was requested without the ApiToken header.");
            context.Response.StatusCode = 401; 
            await context.Response.WriteAsync("Missing ApiToken header.");
            return;
        }

        if (context.Request.Headers["ApiToken"] != expectedToken)
        {
            Console.WriteLine($"{context.Request.Path} was requested with an invalid ApiToken: {context.Request.Headers["ApiToken"]}");
            context.Response.StatusCode = 401; 
            await context.Response.WriteAsync("Invalid ApiToken.");
            return;
        }

        var requestPath = context.Request.Path;
        if (!_allowedPaths.Any(path => requestPath.StartsWithSegments(path)))
        {
            Console.WriteLine($"{requestPath} is not allowed for the Floor Manager.");
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access denied. Floor Manager is not authorized for this path.");
            return;
        }

        await next();
    }
}
