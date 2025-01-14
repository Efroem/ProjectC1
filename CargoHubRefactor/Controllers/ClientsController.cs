using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoHubRefactor.Controllers
{
    [ServiceFilter(typeof(Filters))]
    [Route("api/v1/Clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // Endpoint for paginated clients: limit and page
        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be greater than 0.");
            }

            if (page <= 0)
            {
                return BadRequest("Page number must be greater than 0.");
            }

            var clients = await _clientService.GetClientsPagedAsync(limit, page);

            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found for the given parameters.");
            }

            return Ok(clients);
        }

        // Endpoint for limiting number of clients
        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be greater than 0.");
            }

            var clients = await _clientService.GetClientsAsync(limit);
            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found.");
            }

            return Ok(clients);
        }

        // Get all clients without any limit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _clientService.GetClientsAsync();
            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found.");
            }

            return Ok(clients);
        }

        // Get a single client by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _clientService.GetClientAsync(id);
            if (client == null)
            {
                return NotFound($"Client with ID: {id} not found.");
            }

            return Ok(client);
        }

        // Add a new client
        [HttpPost]
        public async Task<ActionResult<Client>> AddClient([FromBody] Client client)
        {
            if (IsClientInvalid(client))
            {
                return BadRequest("Please provide values for all required fields.");
            }

            var existingClients = await _clientService.GetClientsAsync();
            if (existingClients.Any(x => x.ContactEmail == client.ContactEmail))
            {
                return BadRequest("A client with this email already exists.");
            }

            var newClient = await _clientService.AddClientAsync(client.Name, client.Address, client.City, client.ZipCode,
                                                                client.Province, client.Country, client.ContactName,
                                                                client.ContactPhone, client.ContactEmail);
            return Ok(newClient);
        }

        // Update an existing client
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (IsClientInvalid(client))
            {
                return BadRequest("Please provide values for all required fields.");
            }

            var existingClients = await _clientService.GetClientsAsync();
            if (existingClients.Any(x => x.ContactEmail == client.ContactEmail && x.ClientId != id))
            {
                return BadRequest("A client with this email already exists.");
            }

            var updatedClient = await _clientService.UpdateClientAsync(id, client.Name, client.Address, client.City,
                                                                    client.ZipCode, client.Province, client.Country,
                                                                    client.ContactName, client.ContactPhone,
                                                                    client.ContactEmail);

            if (updatedClient == null)
            {
                return NotFound($"Client with ID: {id} not found.");
            }

            return Ok(updatedClient);
        }

        // Delete a client by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var isDeleted = await _clientService.DeleteClientAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Client with ID: {id} not found.");
            }

            return Ok($"Client with ID: {id} successfully deleted.");
        }

        // Helper method to validate a client
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
}