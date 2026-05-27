using FluentValidation;

namespace ERMS.Application.Tasks.Queries
{
    public class GetTasksByProjectQueryValidator : AbstractValidator<GetTasksByProjectQuery>
    {
        public GetTasksByProjectQueryValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required.");
        }
    }
}
