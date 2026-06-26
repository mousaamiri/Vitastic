using System.Data;
using FluentValidation;

namespace Vitastic.App.Features.Categories.Queries.GetById;

public sealed class GetCategoryByIdValidation:AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdValidation()
    {
        // id canot be empty guid
        RuleFor(c=>c.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته بندی نمی تواند مقدار پیشفرض باشد")
            .NotEmpty().WithMessage("شناسه دسته بندی نمی تواند خالی باشد");
    }
}
