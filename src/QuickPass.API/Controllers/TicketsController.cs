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
                return Ok(await _ticketService.GetAssignedAsync(AccountId));
            }

            return Ok(await _ticketService.GetMineAsync(AccountId));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtener ticket por ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
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
        [SwaggerOperation(Summary = "Auto-asignarse ticket")]
        public async Task<IActionResult> Claim(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.AssignTechAsync(id, AccountId, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/unclaim")]
        [Authorize(Roles = "Tecnico,Administrador")]
        [SwaggerOperation(Summary = "Auto-desasignarse ticket")]
        public async Task<IActionResult> Unclaim(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.UnAssignTech(id, AccountId, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Tecnico,Administrador")]
        [SwaggerOperation(Summary = "Resolver ticket")]
        public async Task<IActionResult> Resolve(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ResolveAsync(id, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Asignar técnico manualmente (Admin)")]
        public async Task<IActionResult> AssignToTech(Guid id, [FromBody] AssignTechRequest request)
        {
            await _ticketService.AssignTechAsync(id, request.TechId, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/force-close")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Forzar cierre (Admin)")]
        public async Task<IActionResult> ForceClose(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment ?? "Cierre forzado por Admin");
            return NoContent();
        }

        [HttpPut("{id}/close")]
        [SwaggerOperation(Summary = "Cerrar ticket (Usuario)")]
        public async Task<IActionResult> Close(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.CloseAsync(id, AccountId, request.Comment);
            return NoContent();
        }

        [HttpPut("{id}/reopen")]
        [SwaggerOperation(Summary = "Reabrir ticket (Usuario)")]
        public async Task<IActionResult> Reopen(Guid id, [FromBody] TicketActionRequest request)
        {
            await _ticketService.ReopenAsync(id, AccountId, request.Comment);
            return NoContent();
        }

        [HttpGet("{id}/history")]
        [SwaggerOperation(Summary = "Obtener historial y comentarios del ticket")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket.CustomerId != AccountId && !IsAdmin && !IsTech)
            {
                return Forbid();
            }

            var history = await _ticketService.GetHistoryAsync(id);
            return Ok(history);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Modificar un ticket existente (Solo creador si está abierto)")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTRequest request)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket.CustomerId != AccountId)
            {
                return Forbid();
            }

            var updated = await _ticketService.UpdateAsync(id, request, AccountId);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Eliminar un ticket (Solo creador si está abierto, o Administrador)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _ticketService.DeleteAsync(id, AccountId, IsAdmin);
            return NoContent();
        }
    }
}