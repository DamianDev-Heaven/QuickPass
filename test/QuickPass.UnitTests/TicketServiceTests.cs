using Moq;
using QuickPass.Application.Contracts.Persistence;
using QuickPass.Application.DTOs.Tickets;
using QuickPass.Application.Services;
using QuickPass.Domain.Entities;
using QuickPass.Domain.Exceptions;
using Xunit;

namespace QuickPass.UnitTests
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepoMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketRepoMock = new Mock<ITicketRepository>();
            _ticketService = new TicketService(_ticketRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTicketResponse_WhenValidRequest()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var request = new CreateTRequest
            {
                Title = "Test Ticket",
                Description = "Test Description",
                Priority = TicketPriority.Alta,
                Category = TicketCategory.Software
            };

            var ticket = new Ticket
            {
                TicketsId = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CustomerId = customerId,
                Status = TicketStatus.Abierto,
                Priority = request.Priority,
                Category = request.Category
            };

            _ticketRepoMock.Setup(x => x.AddAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.CreateAsync(request, customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.TicketsId, result.Id);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(TicketStatus.Abierto.ToString(), result.Status);
            _ticketRepoMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTicketDoesNotExist()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var modifiedBy = Guid.NewGuid();
            var request = new UpdateTRequest { Title = "New Title" };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync((Ticket?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _ticketService.UpdateAsync(ticketId, request, modifiedBy));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenTicketIsNotOpen()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new UpdateTRequest { Title = "New Title" };
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Status = TicketStatus.Asignado // NOT Abierto
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _ticketService.UpdateAsync(ticketId, request, customerId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwner()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var request = new UpdateTRequest { Title = "New Title" };
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Status = TicketStatus.Abierto
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _ticketService.UpdateAsync(ticketId, request, anotherUserId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTicketAndRegisterHistory_WhenValidRequest()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new UpdateTRequest
            {
                Title = "New Title",
                Description = "New Description",
                Priority = TicketPriority.Critica,
                Category = TicketCategory.Hardware
            };
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Title = "Old Title",
                Description = "Old Description",
                Status = TicketStatus.Abierto,
                Priority = TicketPriority.Baja,
                Category = TicketCategory.General
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            _ticketRepoMock.Setup(x => x.UpdateAsync(ticket, customerId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ticketService.UpdateAsync(ticketId, request, customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Title", ticket.Title);
            Assert.Equal("New Description", ticket.Description);
            Assert.Equal(TicketPriority.Critica, ticket.Priority);
            Assert.Equal(TicketCategory.Hardware, ticket.Category);
            _ticketRepoMock.Verify(x => x.UpdateAsync(ticket, customerId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTicketDoesNotExist()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var modifiedBy = Guid.NewGuid();

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync((Ticket?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _ticketService.DeleteAsync(ticketId, modifiedBy, false));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwnerAndNotAdmin()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Status = TicketStatus.Abierto
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _ticketService.DeleteAsync(ticketId, anotherUserId, false));
        }

        [Fact]
        public async Task DeleteAsync_ShouldAllowDelete_WhenUserIsAdminEvenIfNotOwner()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Status = TicketStatus.Asignado // Not Abierto, but Admin can delete
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);
            _ticketRepoMock.Setup(x => x.DeleteAsync(ticketId))
                .Returns(Task.CompletedTask);

            // Act
            await _ticketService.DeleteAsync(ticketId, adminId, true);

            // Assert
            _ticketRepoMock.Verify(x => x.DeleteAsync(ticketId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenUserIsOwnerAndTicketIsOpen()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var ticket = new Ticket
            {
                TicketsId = ticketId,
                CustomerId = customerId,
                Status = TicketStatus.Abierto
            };

            _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);
            _ticketRepoMock.Setup(x => x.DeleteAsync(ticketId))
                .Returns(Task.CompletedTask);

            // Act
            await _ticketService.DeleteAsync(ticketId, customerId, false);

            // Assert
            _ticketRepoMock.Verify(x => x.DeleteAsync(ticketId), Times.Once);
        }
    }
}
