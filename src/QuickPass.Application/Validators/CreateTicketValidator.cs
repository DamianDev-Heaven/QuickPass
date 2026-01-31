using FluentValidation;
using QuickPass.Application.DTOs.Tickets;

namespace QuickPass.Application.Validators;

public class CreateTicketValidator : AbstractValidator<CreateTRequest>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(100).WithMessage("El título no puede exceder los 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("La prioridad seleccionada no es válida.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("La categoría seleccionada no es válida.");
    }
}