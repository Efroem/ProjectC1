using CargoHubRefactor.DbSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Services;

namespace CargoHubRefactor
{
    public class Program
    {
        public static async Task Main(string[] args) // Make Main async
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            // Register the DbContext
            builder.Services.AddDbContext<CargoHubDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("CargoHubDb")));

            // Register services
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IItemGroupService, ItemGroupService>();
            builder.Services.AddScoped<IItemLineService, ItemLineService>();
            builder.Services.AddScoped<IItemTypeService, ItemTypeService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IShipmentService, ShipmentService>();
            builder.Services.AddScoped<SetupItems>();

            // Add health checks
            builder.Services.AddHealthChecks();  // Registers health check services


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHealthChecks("/api/health");  // This maps the /api/health endpoint to check app health


            // Execute SetupItems logic within a valid scope
            using (var scope = app.Services.CreateScope())
            {
                var setupItems = scope.ServiceProvider.GetRequiredService<SetupItems>();

                // Ensure GetItemCategoryRelations is awaited if it’s asynchronous
                await setupItems.GetItemCategoryRelations();
            }

            await app.RunAsync(); // Use RunAsync to work with async Main
        }
    }
}
