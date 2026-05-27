using FluentValidation;

namespace ERMS.Application.Tasks.Queries
{
    public class GetTasksByUserQueryValidator : AbstractValidator<GetTasksByUserQuery>
    {
        public GetTasksByUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
