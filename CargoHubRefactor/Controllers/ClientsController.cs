using Microsoft.AspNetCore.Mvc;

namespace CargoHubRefactor.Controllers {

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

        [HttpGet("limit/{limit}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients(int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show clients with a limit below 1.");
            }

            var clients = await _clientService.GetClientsAsync(limit);
            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found.");
            }

            return Ok(clients);
        }

        [HttpGet("limit/{limit}/page/{page}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsPaged(int limit, int page)
        {
            if (limit <= 0)
            {
                return BadRequest("Cannot show clients with a limit below 1.");
            }
            if (page < 0) return BadRequest("Page number must be a positive integer");

            var clients = await _clientService.GetClientsPagedAsync(limit, page);
            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found.");
            }

            return Ok(clients);
        }


        [HttpPost]
        public async Task<ActionResult<Client>> AddClient([FromBody] Client client)
        {

            if (!client.ContactEmail.Contains('@'))
            {
                return BadRequest("Please provide a valid email address.");
            }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (string.IsNullOrEmpty(client.ContactEmail) || !client.ContactEmail.Contains('@'))
            {
                return BadRequest("Please provide a valid email address.");
            }
            
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
        [HttpDelete("{id}/test")]
        public async Task<IActionResult> SoftDeleteClient(int id) 
        {
            var isDeleted = await _clientService.SoftDeleteClientAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Client with ID: {id} not found.");
            }

            return Ok($"Client with ID: {id} successfully soft deleted.");
        }

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