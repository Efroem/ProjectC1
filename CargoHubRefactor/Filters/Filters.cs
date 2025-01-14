using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

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

        // Retrieve API tokens from configuration
        var adminToken = _configuration["ApiKeys:AdminApiToken"];
        var employeeToken = _configuration["ApiKeys:EmployeeApiToken"];
        var floorManagerToken = _configuration["ApiKeys:FloorManagerApiToken"];
        var warehouseManagerToken = _configuration["ApiKeys:WarehouseManagerToken"];

        // Validate that tokens are properly configured
        if (string.IsNullOrEmpty(adminToken) || string.IsNullOrEmpty(employeeToken) ||
            string.IsNullOrEmpty(floorManagerToken) || string.IsNullOrEmpty(warehouseManagerToken))
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsync("API tokens are not properly configured.");
            return;
        }

        // Validate the presence of the ApiToken header
        if (!httpContext.Request.Headers.ContainsKey("ApiToken"))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized: Missing ApiToken header.");
            return;
        }

        var apiToken = httpContext.Request.Headers["ApiToken"].ToString();
        var requestPath = httpContext.Request.Path;
        var requestMethod = httpContext.Request.Method;

        // Authorization logic
        if (apiToken == adminToken)
        {
            // Admin has full access
            await next();
            return;
        }

        if (apiToken == employeeToken)
        {
            // Employee can only perform GET requests
            if (HttpMethods.IsGet(requestMethod))
            {
                await next();
                return;
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsync("Forbidden: Employee is only authorized for GET requests.");
                return;
            }
        }

        if (apiToken == floorManagerToken)
        {
            // Floor Manager can only perform PUT requests on allowed paths
            if (HttpMethods.IsPut(requestMethod) && _floorManagerAllowedPaths.Any(path => requestPath.StartsWithSegments(path)))
            {
                await next();
                return;
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsync("Forbidden: Floor Manager is not authorized for this request.");
                return;
            }
        }

        if (apiToken == warehouseManagerToken)
        {
            // Warehouse Manager can only perform PUT requests on allowed paths
            if (HttpMethods.IsPut(requestMethod) && _warehouseManagerAllowedPaths.Any(path => requestPath.StartsWithSegments(path)))
            {
                await next();
                return;
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsync("Forbidden: Warehouse Manager is not authorized for this request.");
                return;
            }
        }

        // Invalid or unrecognized token
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        await httpContext.Response.WriteAsync("Forbidden: Invalid or unauthorized ApiToken.");
    }
}
