using FluentValidation;

namespace Vitastic.App.Features.Instructors.Queries.GetById;

public sealed class GetInstructorByIdQueryValidation : AbstractValidator<GetInstructorByIdQuery>
{
    public GetInstructorByIdQueryValidation()
    {
        RuleFor(x => x.InstructorId)
            .NotEqual(Guid.Empty)
            .WithMessage("هیچ استادی با این شناسه یافت نشد. ");
    }
}
