using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.AddTag
{
    public sealed class AddCourseTagCommandHandler
        (ICourseRepository courseRepository,
            ITagRepository tagRepository,
            ICourseTagRepository courseTagRepository)
        : ICommandHandler<AddCourseTagCommand>
    {

        public async Task<Result> Handle(AddCourseTagCommand request, CancellationToken cancellationToken)
        {
            var tagId = TagId.CreateFrom(request.TagId);
            if (tagId.IsFailure)
                return tagId.Error;

            var tagIsExist = await tagRepository.IsExistAsync(tagId.Value, cancellationToken);
            if (!tagIsExist)
                return Error.NotFound("AddCourseTagCommand.TagNotFound", "دوره مورد نظر یافت نشد");

            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;

            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);
            if (!courseIsExist)
                return Error.NotFound("AddCourseTagCommand.NotFound", "دوره مورد نظر یافت نشد");

            var connectionIsExist = courseTagRepository
                .RelationIsExist(courseIdResult.Value,tagId.Value,cancellationToken).Result;
            if(connectionIsExist)
                return Error.Conflict("AddCourseTagCommand.TagAlreadyAssigned", "دوره قبلا به این دسته بندی متصل شده است.");

            var relation = CourseTag.Create(courseIdResult.Value, tagId.Value);
            await courseTagRepository.AddAsync(relation, cancellationToken);
            return Result.Success();
        }
    }
}
