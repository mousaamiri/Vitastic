using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.RemoveTag
{
    public sealed class RemoveCourseTagCommandHandler
        (ICourseRepository courseRepository,
            ITagRepository tagRepository,
            ICourseTagRepository courseTagRepository)
        : ICommandHandler<RemoveCourseTagCommand>
    {

        public async Task<Result> Handle(RemoveCourseTagCommand request, CancellationToken cancellationToken)
        {
            var tagId = TagId.CreateFrom(request.TagId);
            if (tagId.IsFailure)
                return tagId.Error;

            var tagIsExist = await tagRepository.IsExistAsync(tagId.Value, cancellationToken);
            if (!tagIsExist)
                return Error.NotFound("RemoveCourseTagCommand.TagNotFound", "دوره مورد نظر یافت نشد");

            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;

            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);
            if (!courseIsExist)
                return Error.NotFound("RemoveCourseTagCommand.NotFound", "دوره مورد نظر یافت نشد");

            var connectionIsExist = courseTagRepository
                .RelationIsExist(courseIdResult.Value,tagId.Value,cancellationToken).Result;
            if(!connectionIsExist)
                return Error.NotFound("RemoveCourseTagCommand.CourseTagRelationNoFound",
                    "اتصال بین دوره و برچسب یافت نشد.");

            var relation =await courseTagRepository
                .FindAsync(courseIdResult.Value, tagId.Value,cancellationToken);
            await courseTagRepository.DeleteAsync(relation, cancellationToken);
            return Result.Success();
        }
    }
}
