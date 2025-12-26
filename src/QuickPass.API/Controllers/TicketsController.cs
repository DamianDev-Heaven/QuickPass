using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Persistence;
using QuickPass.Domain.Entities;

namespace QuickPass.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _repo;
        public TicketsController(ITicketRepository repo)
        {
            _repo = repo;
        }
        [HttpPost]
        public async Task<IActionResult> CrearTicket()
        {
            var ticket = new Ticket
            {
                TicketsId = Guid.NewGuid(),
                Title = "1er",
                Description = "test",
                CustomerId = Guid.NewGuid(),
                Status = TicketStatus.Abierto
            };
            var result = await _repo.AddAsync(ticket);
            return Ok(result);
        }
    }
}
