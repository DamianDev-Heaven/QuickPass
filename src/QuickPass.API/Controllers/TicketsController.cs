using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickPass.API.Controllers
{
    [Route("api/tickets")]
    [Authorize]
    public class TicketsController : BaseApiController
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtener tickets (Contextual)", Description = "Devuelve tickets según el rol: Admin (Todos), Técnico (Asignados), Usuario (Propios).")]
        public async Task<IActionResult> GetTickets()
        {
            if (IsAdmin)
            {
                return Ok(await _ticketService.GetAllAsync());
            }

            if (IsTech)
            {
                var allAssigned = await _ticketService.GetAssignedAsync(AccountId);
                var myAssigned = allAssigned.Where(t => t.TechId == AccountId).ToList();
                return Ok(myAssigned);
            }

            return Ok(await _ticketService.GetMineAsync(AccountId));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtener ticket por ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            if (ticket.CustomerId != AccountId && !IsAdmin && !IsTech)
            {
                return Forbid();
            }
            return Ok(ticket);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Crear nuevo ticket")]
        public async Task<IActionResult> Create([FromBody] CreateTRequest request)
        {
            var response = await _ticketService.CreateAsync(request, AccountId);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}/claim")]
        [Authorize(Roles = "Tecnico,Administrador")]
        [SwaggerOperation(Summary = "Auto-asignarse ticket (Técnicos)", Description = "El técnico actual toma responsabilidad del ticket.")]
        public async Task<IActionResult> Claim(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.AssignTechAsync(id, AccountId, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Tecnico,Administrador")]
        [SwaggerOperation(Summary = "Resolver ticket (Técnicos)", Description = "Marca el ticket como resuelto.")]
        public async Task<IActionResult> Resolve(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ResolveAsync(id, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Asignar técnico manualmente (Admin)", Description = "Fuerza la asignación de un ticket a un técnico específico.")]
        public async Task<IActionResult> AssignToTech(Guid id, [FromBody] AssignTechRequest request)
        {
            if (request.TechId == Guid.Empty) return BadRequest("ID de técnico inválido.");

            await _ticketService.AssignTechAsync(id, request.TechId, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/force-close")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Forzar cierre (Admin)", Description = "Cierra un ticket administrativamente, ignorando el flujo normal.")]
        public async Task<IActionResult> ForceClose(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment ?? "Cierre forzado por Admin");
            return NoContent();
        }

        [HttpPut("{id}/close")]
        [SwaggerOperation(Summary = "Cerrar ticket (Usuario)", Description = "El usuario confirma que su problema fue solucionado.")]
        public async Task<IActionResult> Close(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/reopen")]
        [SwaggerOperation(Summary = "Reabrir ticket (Usuario)", Description = "El usuario reabre un ticket si el problema persiste.")]
        public async Task<IActionResult> Reopen(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ReopenAsync(id, AccountId, request.Comment);
            return NoContent();
        }
    }
}
