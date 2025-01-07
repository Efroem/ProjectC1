public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Enable buffering to read the request body multiple times
        context.Request.EnableBuffering();

        // Read the body to a string
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

        // Log the body
        _logger.LogInformation("Request Body: {RequestBody}", body);

        // Rewind the request body so the next middleware can read it
        context.Request.Body.Position = 0;

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
