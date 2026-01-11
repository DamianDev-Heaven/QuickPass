using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;

namespace QuickPass.API.Controllers
{
    [Route("api/my-tickets")]
    [Authorize]
    public class TicketsController : BaseApiController
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTRequest request)
        {
            var response = await _ticketService.CreateAsync(request, AccountId);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            var tickets = await _ticketService.GetMineAsync(AccountId);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
                return NotFound();
            if (ticket.CustomerId != AccountId && !IsAdmin && !IsTech)
            {
                return Forbid();
            }
            return Ok(ticket);
        }
    }
}