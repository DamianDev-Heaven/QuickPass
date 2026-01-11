using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;

namespace QuickPass.API.Controllers
{
    [Route("api/tech-tickets")]
    [Authorize(Roles = "Tecnico,Administrador")]
    public class TechTicketsController : BaseApiController
    {
        private readonly ITicketService _ticketService;

        public TechTicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        [HttpPut("{id}/claim")]
        public async Task<IActionResult> AssignToMe(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.AssignTechAsync(id, AccountId, AccountId, request.Comment);
            return NoContent();
        }
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ResolveAsync(id, AccountId, request.Comment);
            return NoContent();
        }
        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment);
            return NoContent();
        }
        [HttpPut("{id}/reopen")]
        public async Task<IActionResult> ReOpen(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ReopenAsync(id, AccountId, request.Comment);
            return NoContent();
        }
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedToMe()
        {
            var allMyTickets = await _ticketService.GetAssignedAsync(AccountId);
            var assigned = allMyTickets.Where(t => t.TechId == AccountId).ToList();
            return Ok(assigned);
        }
    }
}
