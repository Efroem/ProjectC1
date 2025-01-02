public interface IClientService
{
    IEnumerable<Client> GetClients();
    IEnumerable<Client> GetClients(int limit);
    Client GetClient(int id);
    Client AddClient(string name, string address, string city, string zipCode, string province, string country,
                     string contactName, string contactPhone, string contactEmail);
    Client UpdateClient(int id, string name, string address, string city, string zipCode, string province, string country,
                        string contactName, string contactPhone, string contactEmail);
    bool DeleteClient(int id);
}
