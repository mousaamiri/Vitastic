using FluentValidation;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandValidation : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی الزامی است")
            .MinimumLength(CategoryName.MinLength).WithMessage($"نام باید حداقل {CategoryName.MinLength} کاراکتر باشد")
            .MaximumLength(CategoryName.MaxLength).WithMessage($"نام نمی‌تواند بیش از {CategoryName.MaxLength} کاراکتر باشد");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug الزامی است")
            .Matches(Slug.Pattern).WithMessage("Slug فقط می‌تواند حروف کوچک انگلیسی، اعداد و خط تیره داشته باشد")
            .MinimumLength(Slug.MinLength).WithMessage($"نامک باید حداقل {Slug.MinLength} داشته باشد.")
            .MaximumLength(Slug.MaxLength).WithMessage($"نامک باید حداکثر {Slug.MaxLength} داشته باشد.");

        When(x => x.DisplayOrder.HasValue,()=>
        {
            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0).WithMessage("ترتیب نمایش باید بیشتر از صفر باشد");
        });
        When(x => !string.IsNullOrWhiteSpace(x.Slug), () =>
        {

            RuleFor(x => x.Description)
                .MaximumLength(Description.MaxLength)
                .WithMessage($"توضیحات نمی‌تواند بیش از {Description.MaxLength} کاراکتر باشد");
        });

    }
}
