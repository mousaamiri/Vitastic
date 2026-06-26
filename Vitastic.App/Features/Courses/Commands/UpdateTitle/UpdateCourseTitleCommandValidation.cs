using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateTitle
{
    public sealed class UpdateCourseTitleCommandValidation : AbstractValidator<UpdateCourseTitleCommand>
    {
        public UpdateCourseTitleCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان دوره نمی تواند خالی باشد.")
                .MaximumLength(CourseTitle.MaxLength).WithMessage($"عنوان دوره نمی تواند بیشتر از {CourseTitle.MaxLength} کاراکتر باشد.")
                .MinimumLength(CourseTitle.MinLength).WithMessage($"عنوان دوره نمی تواند کمتر از {CourseTitle.MinLength} کاراکتر باشد.");
        }
    }
}