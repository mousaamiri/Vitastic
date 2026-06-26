using FluentValidation;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.UpsertCourseSections
{
    public sealed class SectionDtoValidator : AbstractValidator<SectionDto>
    {
        public SectionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("عنوان بخش الزامی است")
                .MaximumLength(200)
                .WithMessage("عنوان بخش نباید بیشتر از 200 کاراکتر باشد");

            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0)
                .WithMessage("ترتیب نمایش باید بزرگتر از صفر باشد");

            RuleFor(x => x.Episodes)
                .NotNull()
                .WithMessage("لیست اپیزودها نمی‌تواند خالی باشد")
                .Must(episodes => episodes.Count > 0)
                .WithMessage("هر بخش باید حداقل یک اپیزود داشته باشد");

            RuleForEach(x => x.Episodes)
                .SetValidator(new EpisodeDtoValidator());

            // بررسی یکتا بودن DisplayOrder در سطح Episode
            RuleFor(x => x.Episodes)
                .Must(episodes => episodes.Select(e => e.DisplayOrder).Distinct().Count() == episodes.Count)
                .WithMessage("ترتیب نمایش اپیزودها در یک بخش نباید تکراری باشد")
                .When(x => x.Episodes != null && x.Episodes.Count > 0);
        }
    }
}