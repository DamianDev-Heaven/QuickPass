using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;
using System.Security.Claims;

namespace QuickPass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTRequest request)
        {
            var accountId = GetAccountIdFromToken();
            var response = await _ticketService.CreateAsync(request, accountId);
            return Ok(response);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyTickets()
        {
            var accountId = GetAccountIdFromToken();
            var tickets = await _ticketService.GetMineAsync(accountId);
            return Ok(tickets);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() // Funciones para admin 
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }
        private Guid GetAccountIdFromToken()
        {
            var accountIdClaim = User.FindFirstValue("accountId");
            if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out var accountId))
            {
                throw new UnauthorizedAccessException("Token inválido: falta accountId");
            }
            return accountId;
        }
    }
}