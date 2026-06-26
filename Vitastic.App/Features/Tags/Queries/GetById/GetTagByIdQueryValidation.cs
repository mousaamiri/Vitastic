using FluentValidation;

namespace Vitastic.App.Features.Tags.Queries.GetById;

public sealed class GetTagByIdQueryValidation : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidation()
    {
        RuleFor(query => query.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.");
    }
}
