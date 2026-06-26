using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateDescription
{
    public sealed class UpdateCourseDescriptionCommandValidation : AbstractValidator<UpdateCourseDescriptionCommand>
    {
        public UpdateCourseDescriptionCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("توضیحات نمی تواند خالی باشد.")
                .MaximumLength(Description.MaxLength).WithMessage($"توضیحات نمی تواند بیشتر از {Description.MaxLength} کاراکتر باشد.")
                .MinimumLength(Description.MinLength).WithMessage($"توضیحات نمی تواند کمتر از {Description.MinLength} کاراکتر باشد.");

            RuleFor(x => x.ShortDescription)
                .NotEmpty().WithMessage("توضیحات کوتاه نمی تواند خالی باشد.")
                .MinimumLength(ShortDescription.MinLength).WithMessage($"توضیحات کوتاه باید حداقل {ShortDescription.MinLength} کاراکتر باشد.")
                .MaximumLength(ShortDescription.MaxLength).WithMessage($"توضیحات کوتاه نمی تواند بیشتر از {ShortDescription.MaxLength} کاراکتر باشد.");
        }
    }
}