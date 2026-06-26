using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.EnableCertificate
{
    public sealed class EnableCourseCertificateCommandValidation : AbstractValidator<EnableCourseCertificateCommand>
    {
        public EnableCourseCertificateCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}