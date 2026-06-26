using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.EnableCertificate
{
    public sealed record EnableCourseCertificateCommand(
        Guid CourseId
    ) : ICommand;
}