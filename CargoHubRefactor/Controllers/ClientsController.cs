using Microsoft.AspNetCore.Mvc;

[ServiceFilter(typeof(AdminFilter))]
[Route("api/v1/Clients")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("limit/{limit}")]
    public ActionResult<IEnumerable<Client>> GetClients(int limit)
    {
        if (limit <= 0)
        {
            return BadRequest("Cant show id below 0.");
        }

        var clients = _clientService.GetClients(limit);
        if (clients == null || !clients.Any())
        {
            return NotFound("No clients found.");
        }

        return Ok(clients);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Client>> GetClients()
    {
        var clients = _clientService.GetClients();
        if (clients == null || !clients.Any())
        {
            return NotFound("No clients found.");
        }

        return Ok(clients);
    }

    [HttpGet("{id}")]
    public ActionResult<Client> GetClient(int id)
    {
        var client = _clientService.GetClient(id);
        if (client == null)
        {
            return NotFound($"Client with ID: {id} not found.");
        }

        return Ok(client);
    }

    [HttpPost]
    public ActionResult<Client> AddClient([FromBody] Client client)
    {
        if (IsClientInvalid(client))
        {
            return BadRequest("Please provide values for all required fields.");
        }

        if (_clientService.GetClients().Any(x => x.ContactEmail == client.ContactEmail))
        {
            return BadRequest("A client with this email already exists.");
        }

        var newClient = _clientService.AddClient(client.Name, client.Address, client.City, client.ZipCode, client.Province,
                                                 client.Country, client.ContactName, client.ContactPhone, client.ContactEmail);
        return Ok(newClient);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateClient(int id, [FromBody] Client client)
    {
        if (IsClientInvalid(client))
        {
            return BadRequest("Please provide values for all required fields.");
        }

        if (_clientService.GetClients().Any(x => x.ContactEmail == client.ContactEmail && x.ClientId != id))
        {
            return BadRequest("A client with this email already exists.");
        }

        var updatedClient = _clientService.UpdateClient(id, client.Name, client.Address, client.City, client.ZipCode, client.Province,
                                                        client.Country, client.ContactName, client.ContactPhone, client.ContactEmail);

        if (updatedClient == null)
        {
            return NotFound($"Client with ID: {id} not found.");
        }

        return Ok(updatedClient);
    }

    // Delete a client
    [HttpDelete("{id}")]
    public IActionResult DeleteClient(int id)
    {
        var isDeleted = _clientService.DeleteClient(id);
        if (!isDeleted)
        {
            return NotFound($"Client with ID: {id} not found.");
        }

        return Ok($"Client with ID: {id} successfully deleted");
    }

    // Helper method to validate client properties
    private bool IsClientInvalid(Client client)
    {
        return string.IsNullOrEmpty(client.Name) ||
               string.IsNullOrEmpty(client.Address) ||
               string.IsNullOrEmpty(client.City) ||
               string.IsNullOrEmpty(client.ZipCode) ||
               string.IsNullOrEmpty(client.Province) ||
               string.IsNullOrEmpty(client.Country) ||
               string.IsNullOrEmpty(client.ContactName) ||
               string.IsNullOrEmpty(client.ContactPhone) ||
               string.IsNullOrEmpty(client.ContactEmail);
    }
}
