public interface IClientService
{
    Task<IEnumerable<Client>> GetClientsAsync();
    Task<IEnumerable<Client>> GetClientsAsync(int limit);
    Task<IEnumerable<Client>> GetClientsPagedAsync(int limit, int page);
    Task<Client> GetClientAsync(int id);
    Task<Client> AddClientAsync(string name, string address, string city, string zipCode, string province, string country,
                                string contactName, string contactPhone, string contactEmail);
    Task<Client> UpdateClientAsync(int id, string name, string address, string city, string zipCode, string province, 
                                   string country, string contactName, string contactPhone, string contactEmail);
    Task<bool> DeleteClientAsync(int id);
    Task<bool> SoftDeleteClientAsync(int id);
}
