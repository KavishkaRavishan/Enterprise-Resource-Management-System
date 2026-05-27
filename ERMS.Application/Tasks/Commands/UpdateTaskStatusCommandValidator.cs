using FluentValidation;

namespace ERMS.Application.Tasks.Commands
{
    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(x => x.Equals("ToDo", StringComparison.OrdinalIgnoreCase) ||
                           x.Equals("InProgress", StringComparison.OrdinalIgnoreCase) ||
                           x.Equals("Done", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Status must be either 'ToDo', 'InProgress', or 'Done'.");
        }
    }
}
