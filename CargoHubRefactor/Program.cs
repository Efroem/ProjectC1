using CargoHubRefactor.DbSetup;
using CargoHubRefactor.Services;
using Microsoft.EntityFrameworkCore;
using Services;

namespace CargoHubRefactor
{
    public class Program
    {
        public static async Task Main(string[] args)
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
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IItemGroupService, ItemGroupService>();
            builder.Services.AddScoped<IItemLineService, ItemLineService>();
            builder.Services.AddScoped<IItemTypeService, ItemTypeService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<ReportingService>();
            builder.Services.AddScoped<IShipmentService, ShipmentService>();
            builder.Services.AddScoped<SetupItems>();
            builder.Services.AddScoped<Filters>();

            // Add health checks
            builder.Services.AddHealthChecks();

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "CargoHub API",
                    Version = "v1",
                    Description = "API documentation for CargoHubRefactor",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Your Name",
                        Email = "your.email@example.com"
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CargoHub API v1");
                });
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHealthChecks("/api/health");

            using (var scope = app.Services.CreateScope())
            {
                var setupItems = scope.ServiceProvider.GetRequiredService<SetupItems>();
                await setupItems.GetItemCategoryRelations();
            }

            await app.RunAsync();
        }
    }
}
