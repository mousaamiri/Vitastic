using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.SetTagList
{
    public sealed class SetCourseTagsCommandHandler(ICourseRepository courseRepository
    ,ICourseTagRepository courseTagRepository)
        : ICommandHandler<SetCourseTagsCommand>
    {
        public async Task<Result> Handle(SetCourseTagsCommand request, CancellationToken cancellationToken)
        {
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);
            if (!courseIsExist)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            IEnumerable<TagId> tagIds = request.TagIds.Select(id => TagId.CreateFrom(id).Value);

            await courseTagRepository.ReassignCourseTagsAsync(courseIdResult.Value,tagIds, cancellationToken);
            return Result.Success();
        }
    }
}
