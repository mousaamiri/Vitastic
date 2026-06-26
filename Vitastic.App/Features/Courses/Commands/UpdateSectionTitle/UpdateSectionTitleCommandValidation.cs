using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionTitle
{
    public sealed class UpdateSectionTitleCommandValidation : AbstractValidator<UpdateSectionTitleCommand>
    {
        public UpdateSectionTitleCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("شناسه بخش نمی تواند خالی باشد.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان بخش نمی تواند خالی باشد.")
                .MaximumLength(SectionTitle.MaxLength).WithMessage($"عنوان بخش نمی تواند بیشتر از {SectionTitle.MaxLength} کاراکتر باشد.")
                .MinimumLength(SectionTitle.MinLength).WithMessage($"عنوان بخش نمی تواند کمتر از {SectionTitle.MinLength} کاراکتر باشد.");
        }
    }
}