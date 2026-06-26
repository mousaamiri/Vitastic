using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.EnableCertificate
{
    public sealed class EnableCourseCertificateCommandHandler : ICommandHandler<EnableCourseCertificateCommand>
    {
        private readonly ICourseRepository _courseRepository;

        public EnableCourseCertificateCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result> Handle(EnableCourseCertificateCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await _courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var result = course.EnableCertificate();
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}