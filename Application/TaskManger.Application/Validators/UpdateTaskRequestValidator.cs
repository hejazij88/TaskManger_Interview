using FluentValidation;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Validators;

public class UpdateTaskRequestValidator
    : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("DueDate must be in the future.");
    }
}