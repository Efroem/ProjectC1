using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.EntityFrameworkCore;

public interface IApiKeyService
{
    Task<string> GetAdminApiTokenAsync();
    Task<string> GetEmployeeApiTokenAsync();
    Task<string> GetFloorManagerApiTokenAsync();
    Task<string> GetWarehouseManagerTokenAsync();
}

public class ApiKeyService : IApiKeyService
{
    private readonly CargoHubDbContext _dbContext;  // Inject the DbContext

    public ApiKeyService(CargoHubDbContext dbContext)
    {
        _dbContext = dbContext;  // Assign DbContext
    }

    public async Task<string> GetAdminApiTokenAsync()
    {
        return await GetTokenAsync("AdminApiToken");
    }

    public async Task<string> GetEmployeeApiTokenAsync()
    {
        return await GetTokenAsync("EmployeeApiToken");
    }

    public async Task<string> GetFloorManagerApiTokenAsync()
    {
        return await GetTokenAsync("FloorManagerApiToken");
    }

    public async Task<string> GetWarehouseManagerTokenAsync()
    {
        return await GetTokenAsync("WarehouseManagerToken");
    }

    public async Task<string> GetEnvTestTokenAsync() {
        return await GetTokenAsync("EnvTestToken");
    }

    private async Task<string> GetTokenAsync(string key)
    {
        // Check if environment specifies to use database
        var apiKeyFromEnv = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrEmpty(apiKeyFromEnv))
        {
            apiKeyFromEnv = HashString(apiKeyFromEnv);
            return apiKeyFromEnv; // If found in the environment, return it.
        }
        else {
            var apiKeyFromDb = _dbContext.APIKeys.FirstOrDefault(x => x.Name == key);
            if (apiKeyFromDb == null) return null;
            return apiKeyFromDb.Key;
        }
    }


    public static string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return Encoding.Default.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(input)));
        }
    }
}
