using CargoHubRefactor.DbSetup;
using CargoHubRefactor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Services;

namespace CargoHubRefactor
{
    public class Program
    {
        static void Main(string[] args)
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
            builder.Services.AddScoped<ReportingService>();
            builder.Services.AddScoped<SetupItems>();
            
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

            using (var scope = app.Services.CreateScope())
                {
                    var setupItems = scope.ServiceProvider.GetRequiredService<SetupItems>();

                    // Call GetItemCategoryRelations method (make sure this method is non-static)
                    var relations = setupItems.GetItemCategoryRelations();
                }
            app.Run();
        }
    }
}
