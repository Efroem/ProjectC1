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

        // LoadHashedKeysInDB("AdminApiToken", "A1B2C3D4");
        // LoadHashedKeysInDB("EmployeeApiToken", "H8I9J10");
        // LoadHashedKeysInDB("FloorManagerApiToken", "E5F6G7");
        // LoadHashedKeysInDB("WarehouseManagerToken", "K11L12M13");

        if (!string.IsNullOrEmpty(apiKeyFromEnv))
        {
            apiKeyFromEnv = HashString(apiKeyFromEnv);
            return apiKeyFromEnv; // If found in the environment, return it.
        }
        else {
            var apiKeyFromDb = _dbContext.APIKeys.FirstOrDefault(x => x.Name == key).Key;
            if (apiKeyFromDb == null) return null;
            return apiKeyFromDb;
        }
        // Fall back to app configuration or environment variables
    }

    private void LoadHashedKeysInDB(string name, string key) {
        var ApiKey = _dbContext.APIKeys.FirstOrDefault(a => a.Name == name);
        if (ApiKey == null) return;
        ApiKey.Key = HashString(key);
        _dbContext.SaveChanges();
    }

    public static string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return Encoding.Default.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(input)));
            // byte[] hashBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(input));
            // return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
