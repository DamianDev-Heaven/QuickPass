using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;

namespace QuickPass.API.Controllers
{
    [Route("api/org-tickets")]
    [Authorize(Roles = "Administrador,Tecnico")]
    public class OrgTicketsController : BaseApiController
    {
        private readonly ITicketService _ticketService;

        public OrgTicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicketToTech(Guid id, [FromBody] AssignTechRequest request)
        {
            if (request.TechId == Guid.Empty)
            {
                return BadRequest("Debes especificar un ID de técnico válido.");
            }
            await _ticketService.AssignTechAsync(id, request.TechId, AccountId, request.Comment);

            return NoContent();
        }

        [HttpPut("{id}/force-close")]
        public async Task<IActionResult> ForceClose(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment ?? "Cierre forzado por Admin");
            return NoContent();
        }
    }
}
