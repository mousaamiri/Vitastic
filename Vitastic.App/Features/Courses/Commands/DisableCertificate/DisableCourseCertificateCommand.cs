using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.DisableCertificate
{
    public sealed record DisableCourseCertificateCommand(
        Guid CourseId
    ) : ICommand;
}