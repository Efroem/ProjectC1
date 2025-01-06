using Microsoft.AspNetCore.Mvc.Filters;

public class AdminFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public AdminFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
    {
        var context = actionContext.HttpContext;

        var expectedToken = _configuration["ApiKeys:AdminApiToken"];

        if (string.IsNullOrEmpty(expectedToken))
        {
            Console.WriteLine("Check om te kijken of AdminFilter wel juist in appsettings staat (error)");
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

        await next();
    }
}
