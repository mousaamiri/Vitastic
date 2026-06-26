using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.DisableCertificate
{
    public sealed class DisableCourseCertificateCommandValidation : AbstractValidator<DisableCourseCertificateCommand>
    {
        public DisableCourseCertificateCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}