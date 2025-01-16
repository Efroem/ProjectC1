using System.Collections.Generic;
using System.Threading.Tasks;

public interface IClientService
{
    Task<IEnumerable<Client>> GetClientsAsync(); // Get all clients without any limit
    Task<IEnumerable<Client>> GetClientsAsync(int limit); // Get clients with a limit
    
    Task<IEnumerable<Client>> GetClientsPagedAsync(int limit, int page);

    Task<Client> GetClientAsync(int id); // Get a single client by ID
    
    Task<Client> AddClientAsync(string name, string address, string city, string zipCode, string province, 
                                string country, string contactName, string contactPhone, string contactEmail); // Add a new client
    
    Task<Client> UpdateClientAsync(int id, string name, string address, string city, string zipCode, string province, 
                                   string country, string contactName, string contactPhone, string contactEmail); // Update an existing client
    
    Task<bool> DeleteClientAsync(int id); // Delete a client by ID
}
