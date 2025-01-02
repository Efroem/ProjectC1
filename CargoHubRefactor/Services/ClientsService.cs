using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ClientService : IClientService
{
    private readonly CargoHubDbContext _context;

    public ClientService(CargoHubDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Client> GetClients(int limit)
    {
        return _context.Clients.Take(limit).ToList();
    }

    public IEnumerable<Client> GetClients()
    {
        return _context.Clients.ToList(); 
    }

    public Client GetClient(int id)
    {
        return _context.Clients.FirstOrDefault(x => x.ClientId == id);
    }

    public Client AddClient(string name, string address, string city, string zipCode, string province, string country,
                        string contactName, string contactPhone, string contactEmail)
    {
        int nextId;

        if (_context.Clients.Any())
        {
            nextId = _context.Clients.Max(c => c.ClientId) + 1;
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

        _context.Clients.Add(client);
        _context.SaveChanges();

        return client;
    }


    public Client UpdateClient(int id, string name, string address, string city, string zipCode, string province, string country,
                               string contactName, string contactPhone, string contactEmail)
    {
        var client = _context.Clients.FirstOrDefault(c => c.ClientId == id);
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
        _context.SaveChanges();

        return client;
    }

    public bool DeleteClient(int id)
    {
        var client = _context.Clients.FirstOrDefault(c => c.ClientId == id);
        if (client == null)
        {
            return false;
        }

        _context.Clients.Remove(client);
        _context.SaveChanges();

        return true;
    }
}
