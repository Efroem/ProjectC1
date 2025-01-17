using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ClientService : IClientService
{
    private readonly CargoHubDbContext _context;

    public ClientService(CargoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetClientsAsync(int limit)
    {
        return await _context.Clients.Take(limit).ToListAsync();
    }
    public async Task<IEnumerable<Client>> GetClientsPagedAsync(int limit, int page)
    {
        return await _context.Clients.Skip(limit * (page - 1)).Take(limit).ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client> GetClientAsync(int id)
    {
        return await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == id);
    }

    public async Task<Client> AddClientAsync(string name, string address, string city, string zipCode, string province,
                                             string country, string contactName, string contactPhone, string contactEmail)
    {
        int nextId;

        if (await _context.Clients.AnyAsync())
        {
            nextId = await _context.Clients.MaxAsync(c => c.ClientId) + 1;
        }
        else
        {
            nextId = 1;
        }

        var client = new Client
        {
            ClientId = nextId,
            Name = name,
            Address = address,
            City = city,
            ZipCode = zipCode,
            Province = province,
            Country = country,
            ContactName = contactName,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        return client;
    }

    public async Task<Client> UpdateClientAsync(int id, string name, string address, string city, string zipCode, 
                                                string province, string country, string contactName, 
                                                string contactPhone, string contactEmail)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
        if (client == null)
        {
            throw new KeyNotFoundException($"Client with ID {id} not found.");
        }

        client.Name = name;
        client.Address = address;
        client.City = city;
        client.ZipCode = zipCode;
        client.Province = province;
        client.Country = country;
        client.ContactName = contactName;
        client.ContactPhone = contactPhone;
        client.ContactEmail = contactEmail;
        client.UpdatedAt = DateTime.Now;

        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        return client;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
        if (client == null)
        {
            return false;
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> SoftDeleteClientAsync(int id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
        if (client == null)
        {
            return false;
        }

        client.SoftDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
